using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Repositories.Entities.Extensions;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep.Audit;

namespace Unite.Mutations.Feed.Data.Services.Annotations
{
    public class VepAnnotationsDataService : IDataService<AnnotationModel, AnnotationsUploadAudit>
    {
        private readonly UniteDbContext _dbContext;

        private readonly ILogger _logger;

        public VepAnnotationsDataService(UniteDbContext dbContext, ILogger<VepAnnotationsDataService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public void SaveData(IEnumerable<AnnotationModel> models, out AnnotationsUploadAudit audit)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                audit = new AnnotationsUploadAudit();

                var annotationModels = Enumerable.Empty<AnnotationModel>();

                annotationModels = models
                    .Where(model => model.AffectedTranscripts != null)
                    .ToArray();

                #region Mutations
                var mutationModels = Enumerable.Empty<Mutation>();

                try
                {
                    mutationModels = annotationModels
                    .Select(annotationModel => annotationModel.Mutation.ToEntity())
                    .ToArray();

                    foreach (var mutationModel in mutationModels)
                    {
                        var mutation = _dbContext.Mutations
                            .First(mutation => mutation.Code == mutationModel.Code);

                        audit.Mutations.Add(mutation.Id);
                    }
                }
                catch
                {
                    _logger.LogError("Could not parse mutations.");

                    throw;
                }
                #endregion

                #region Biotypes
                var biotypeModels = annotationModels
                    .SelectMany(annotationModel => annotationModel.AffectedTranscripts
                        .Select(affectedTranscript => new Biotype { Value = affectedTranscript.Transcript.Biotype }
                    ))
                    .GroupBy(biotypeModel => biotypeModel.Value)
                    .Select(group => group.First())
                    .ToArray();

                var biotypesToAdd = new List<Biotype>();
                foreach (var biotypeModel in biotypeModels)
                {
                    var biotype = _dbContext.Biotypes
                        .FirstOrDefault(biotype => biotype.Value == biotypeModel.Value);

                    if (biotype == null)
                    {
                        biotypesToAdd.Add(biotypeModel);
                    }
                }

                if (biotypesToAdd.Any())
                {
                    _dbContext.Biotypes.AddRange(biotypesToAdd);
                    _dbContext.SaveChanges();

                    audit.BiotypesCreated += biotypesToAdd.Count();
                    biotypesToAdd = null;
                }
                #endregion

                #region Genes
                var geneModels = annotationModels
                    .SelectMany(annotationModel => annotationModel.AffectedTranscripts
                        .Select(affectedTranscript => affectedTranscript.Gene.ToEntity()
                    ))
                    .GroupBy(geneModel => geneModel.Info.EnsemblId)
                    .Select(group => group.First())
                    .ToArray();

                var genesToAdd = new List<Gene>();
                foreach (var geneModel in geneModels)
                {
                    var gene = _dbContext.Genes
                        .Include(gene => gene.Info)
                        .FirstOrDefault(gene => gene.Info.EnsemblId == geneModel.Info.EnsemblId);

                    if (gene == null)
                    {
                        genesToAdd.Add(geneModel);
                    }
                }

                if (genesToAdd.Any())
                {
                    _dbContext.Genes.AddRange(genesToAdd);
                    _dbContext.SaveChanges();

                    audit.GenesCreated += genesToAdd.Count();
                    genesToAdd = null;
                }
                #endregion

                #region Transcripts
                var transcriptModels = annotationModels
                    .SelectMany(annotationModel => annotationModel.AffectedTranscripts
                        .Select(affectedTranscript => affectedTranscript.Transcript.ToEntity()
                            .With(transcriptModel => transcriptModel.Biotype, null)
                            .With(transcriptModel => transcriptModel.BiotypeId, GetBiotypeId(affectedTranscript.Transcript.Biotype))
                    ))
                    .GroupBy(transcriptModel => transcriptModel.Info.EnsemblId)
                    .Select(group => group.First())
                    .ToArray();

                var transcriptsToAdd = new List<Transcript>();
                foreach (var transcriptModel in transcriptModels)
                {
                    var transcript = _dbContext.Transcripts
                        .FirstOrDefault(transcript => transcript.Info.EnsemblId == transcriptModel.Info.EnsemblId);

                    if (transcript == null)
                    {
                        transcriptsToAdd.Add(transcriptModel);
                    }
                }

                if (transcriptsToAdd.Any())
                {
                    _dbContext.Transcripts.AddRange(transcriptsToAdd);
                    _dbContext.SaveChanges();

                    audit.TranscriptsCreated += transcriptsToAdd.Count();
                    transcriptsToAdd = null;
                }
                #endregion

                #region Affected transcripts
                var affectedTranscriptModels = annotationModels
                    .SelectMany(annotationModel => annotationModel.AffectedTranscripts
                        .Select(affectedTranscript => affectedTranscript.ToEntity()
                            .With(affectedTranscriptModel => affectedTranscriptModel.Mutation, null)
                            .With(affectedTranscriptModel => affectedTranscriptModel.MutationId, GetMutationId(annotationModel.Mutation.Code))
                            .With(affectedTranscriptModel => affectedTranscriptModel.Gene, null)
                            .With(affectedTranscriptModel => affectedTranscriptModel.GeneId, GetGeneId(affectedTranscript.Gene.EnsemblId))
                            .With(affectedTranscriptModel => affectedTranscriptModel.Transcript, null)
                            .With(affectedTranscriptModel => affectedTranscriptModel.TranscriptId, GetTranscriptId(affectedTranscript.Transcript.EnsemblId))
                            .With(affectedTranscriptModel => affectedTranscriptModel.Consequences, GetConsequences(affectedTranscript.Consequences))
                    ))
                    .ToArray();

                var affectedTranscriptsToAdd = new List<AffectedTranscript>();
                foreach (var affectedTranscriptModel in affectedTranscriptModels)
                {
                    var affectedTranscript = _dbContext.TranscriptConsequences.FirstOrDefault(affectedTranscript =>
                        affectedTranscript.MutationId == affectedTranscriptModel.MutationId &&
                        affectedTranscript.GeneId == affectedTranscriptModel.GeneId &&
                        affectedTranscript.TranscriptId == affectedTranscriptModel.TranscriptId
                    );

                    if (affectedTranscript == null)
                    {
                        affectedTranscriptsToAdd.Add(affectedTranscriptModel);
                    }
                }

                if (affectedTranscriptsToAdd.Any())
                {
                    _dbContext.TranscriptConsequences.AddRange(affectedTranscriptsToAdd);
                    _dbContext.SaveChanges();

                    audit.AffectedTranscriptsCreated += affectedTranscriptsToAdd.Count();
                    affectedTranscriptsToAdd = null;
                }
                #endregion

                transaction.Commit();
            }
            catch
            {
                audit = null;

                transaction.Rollback();

                throw;
            }
        }


        private int GetMutationId(string code)
        {
            try
            {
                return _dbContext.Mutations
                    .Where(mutation => mutation.Code == code)
                    .Select(mutation => mutation.Id)
                    .First();
            }
            catch
            {
                _logger.LogError($"Mutation with code '{code}' was not found");

                throw;
            }
            
        }

        private int GetBiotypeId(string value)
        {
            try
            {
                return _dbContext.Biotypes
                    .Where(biotype => biotype.Value == value)
                    .Select(biotype => biotype.Id)
                    .First();
            }
            catch
            {
                _logger.LogError($"Biotype with value '{value}' was not found");

                throw;
            }
        }

        private int GetGeneId(string ensemblId)
        {
            try
            {
                return _dbContext.Genes
                    .Where(gene => gene.Info.EnsemblId == ensemblId)
                    .Select(gene => gene.Id)
                    .First();
            }
            catch
            {
                _logger.LogError($"Gene with EnsemblId '{ensemblId}' was not found");

                throw;
            }
        }

        private int GetTranscriptId(string ensemblId)
        {
            try
            {
                return _dbContext.Transcripts
                    .Where(transcript => transcript.Info.EnsemblId == ensemblId)
                    .Select(transcript => transcript.Id)
                    .First();
            }
            catch
            {
                _logger.LogError($"Transcript with EnsemblId '{ensemblId}' was not found");

                throw;
            }
        }

        private AffectedTranscriptConsequence[] GetConsequences(IEnumerable<ConsequenceModel> consequences)
        {
            try
            {
                var consequenceTypes = consequences.Select(consequence => consequence.Type).ToArray();

                return _dbContext.Consequences
                    .ToArray()
                    .Where(consequence => consequenceTypes.Any(typeId => consequence.TypeId == typeId))
                    .Select(consequece => new AffectedTranscriptConsequence { ConsequenceId = consequece.TypeId })
                    .ToArray();
            }
            catch
            {
                _logger.LogError($"Consequences were not found");

                throw;
            }
        }
    }
}
