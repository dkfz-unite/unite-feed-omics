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

                foreach (var model in models)
                {
                    ProcessModel(model, ref audit);
                }

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

                audit.AffectedTranscriptConsequencesAssociated++;
            }

            return entity;
        }
    }
}
