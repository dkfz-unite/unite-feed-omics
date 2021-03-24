using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Repositories;
using Unite.Mutations.Feed.Data.Repositories.Entities.Extensions;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep.Audit;

namespace Unite.Mutations.Feed.Data.Services.Annotations
{
    public class VepAnnotationsDataService : IDataService<AnnotationModel, AnnotationsUploadAudit>
    {
        private readonly UniteDbContext _dbContext;
        private readonly Repository<Mutation> _mutationRepository;
        private readonly Repository<Gene> _geneRepository;
        private readonly Repository<Transcript> _transcriptRepository;
        private readonly Repository<AffectedTranscript> _affectedTranscriptRepository;
        private readonly Repository<AffectedTranscriptConsequence> _affectedTranscriptConsequenceRepository;
        private readonly Repository<Consequence> _consequenceRepository;

        public VepAnnotationsDataService(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _mutationRepository = new MutationRepository(dbContext);
            _geneRepository = new GeneRepository(dbContext);
            _transcriptRepository = new TranscriptRepository(dbContext);
            _affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext);
            _affectedTranscriptConsequenceRepository = new AffectedTranscriptConsequenceRepository(dbContext);
            _consequenceRepository = new ConsequenceRepository(dbContext);
        }

        public void SaveData(AnnotationModel model, out AnnotationsUploadAudit audit)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                audit = new AnnotationsUploadAudit();

                ProcessModel(model, ref audit);

                transaction.Commit();
            }
            catch
            {
                audit = null;

                transaction.Rollback();

                throw;
            }
        }

        public void SaveData(IEnumerable<AnnotationModel> models, out AnnotationsUploadAudit audit)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                audit = new AnnotationsUploadAudit();

                var annotationModels = models.Where(model => model.AffectedTranscripts != null).ToArray();

                #region Mutations
                var mutationModels = annotationModels
                    .Select(annotationModel => annotationModel.Mutation.ToEntity())
                    .ToArray();

                foreach (var mutationModel in mutationModels)
                {
                    var mutation = _dbContext.Mutations
                        .First(mutation => mutation.Code == mutationModel.Code);

                    audit.Mutations.Add(mutation.Id);
                }

                Console.WriteLine("Mutations");
                Console.WriteLine(string.Join(", ", mutationModels.Select(model => model.Code).ToArray()));
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


        private void ProcessModel(AnnotationModel model, ref AnnotationsUploadAudit audit)
        {
            var mutationModel = model.Mutation.ToEntity();
            var mutation = Get(mutationModel);

            if (model.AffectedTranscripts != null)
            {
                foreach (var affectedTranscriptEntry in model.AffectedTranscripts)
                {
                    var geneModel = affectedTranscriptEntry.Gene.ToEntity();
                    var gene = GetOrCreate(geneModel, ref audit);

                    var transcriptModel = affectedTranscriptEntry.Transcript.ToEntity();
                    var transcript = GetOrCreate(transcriptModel, ref audit);

                    var affectedTranscriptModel = affectedTranscriptEntry.ToEntity()
                        .With(entity => entity.Mutation, mutation)
                        .With(entity => entity.Gene, gene)
                        .With(entity => entity.Transcript, transcript);
                    var affectedTranscript = GetOrCreate(affectedTranscriptModel, ref audit);

                    foreach (var consequenceEntry in affectedTranscriptEntry.Consequences)
                    {
                        var consequenceModel = consequenceEntry.ToEntity();
                        var consequence = Get(consequenceModel);

                        var affectedTranscriptConsequenceModel = new AffectedTranscriptConsequence()
                            .With(entity => entity.AffectedTranscript, affectedTranscript)
                            .With(entity => entity.Consequence, consequence);
                        var affectedTranscriptConsequence = GetOrCreate(affectedTranscriptConsequenceModel, ref audit);
                    }
                }
            }

            audit.Mutations.Add(mutation.Id);
        }


        private Mutation Get(in Mutation model)
        {
            var code = model.Code;

            var entity = _mutationRepository.Entities
                .FirstOrDefault(entity =>
                    entity.Code == code
            );

            return entity;
        }

        private Gene GetOrCreate(in Gene model, ref AnnotationsUploadAudit audit)
        {
            var ensemblId = model.Info.EnsemblId;

            var entity = _geneRepository.Entities
                .Include(entity => entity.Info)
                .FirstOrDefault(entity =>
                    entity.Info.EnsemblId == ensemblId
            );

            if (entity == null)
            {
                entity = _geneRepository.Add(model);

                audit.GenesCreated++;
            }

            return entity;
        }

        private Transcript GetOrCreate(in Transcript model, ref AnnotationsUploadAudit audit)
        {
            var ensemblId = model.Info.EnsemblId;

            var entity = _transcriptRepository.Entities
                .Include(entity => entity.Info)
                .FirstOrDefault(entity =>
                    entity.Info.EnsemblId == ensemblId
            );

            if (entity == null)
            {
                entity = _transcriptRepository.Add(model);

                audit.TranscriptsCreated++;
            }

            return entity;
        }

        private AffectedTranscript GetOrCreate(in AffectedTranscript model, ref AnnotationsUploadAudit audit)
        {
            var mutationId = model.Mutation.Id;
            var geneId = model.Gene.Id;
            var transcriptId = model.Transcript.Id;

            var entity = _affectedTranscriptRepository.Entities
                .FirstOrDefault(entity =>
                    entity.MutationId == mutationId &&
                    entity.GeneId == geneId &&
                    entity.TranscriptId == transcriptId
            );

            if (entity == null)
            {
                entity = _affectedTranscriptRepository.Add(model);

                audit.AffectedTranscriptsCreated++;
            }

            return entity;
        }

        private Consequence Get(in Consequence model)
        {
            var typeId = model.TypeId;

            var entity = _consequenceRepository.Entities
                .FirstOrDefault(entity =>
                    entity.TypeId == typeId
            );

            return entity;
        }

        private AffectedTranscriptConsequence GetOrCreate(in AffectedTranscriptConsequence model, ref AnnotationsUploadAudit audit)
        {
            var affectedTranscriptId = model.AffectedTranscript.Id;
            var consequenceId = model.Consequence.TypeId;

            var entity = _affectedTranscriptConsequenceRepository.Entities
                .FirstOrDefault(entity =>
                    entity.AffectedTranscriptId == affectedTranscriptId &&
                    entity.ConsequenceId == consequenceId
            );

            if (entity == null)
            {
                entity = _affectedTranscriptConsequenceRepository.Add(model);
            }

            return entity;
        }


        private int GetMutationId(string code)
        {
            return _dbContext.Mutations
                .Where(mutation => mutation.Code == code)
                .Select(mutation => mutation.Id)
                .First();
        }

        private int GetBiotypeId(string value)
        {
            return _dbContext.Biotypes
                .Where(biotype => biotype.Value == value)
                .Select(biotype => biotype.Id)
                .First();
        }

        private int GetGeneId(string ensemblId)
        {
            return _dbContext.Genes
                .Where(gene => gene.Info.EnsemblId == ensemblId)
                .Select(gene => gene.Id)
                .First();
        }

        private int GetTranscriptId(string ensemblId)
        {
            return _dbContext.Transcripts
                .Where(transcript => transcript.Info.EnsemblId == ensemblId)
                .Select(transcript => transcript.Id)
                .First();
        }

        private AffectedTranscriptConsequence[] GetConsequences(IEnumerable<ConsequenceModel> consequences)
        {
            var consequenceTypes = consequences.Select(consequence => consequence.Type).ToArray();

            return _dbContext.Consequences
                .ToArray()
                .Where(consequence => consequenceTypes.Any(typeId => consequence.TypeId == typeId))
                .Select(consequece => new AffectedTranscriptConsequence { ConsequenceId = consequece.TypeId })
                .ToArray();
        }
    }
}
