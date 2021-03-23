using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Repositories;
using Unite.Mutations.Feed.Data.Repositories.Entities.Extensions;
using Unite.Mutations.Feed.Data.Services.Mutations.Models;
using Unite.Mutations.Feed.Data.Services.Mutations.Models.Audit;

namespace Unite.Mutations.Feed.Data.Services.Mutations
{
    public class MutationsDataService : IDataService<MutationsModel, MutationsUploadAudit>
    {
        private readonly UniteDbContext _dbContext;
        private readonly Repository<Donor> _donorRepository;
        private readonly Repository<Analysis> _analysisRepository;
        private readonly Repository<Sample> _sampleRepository;
        private readonly Repository<AnalysedSample> _analysedSampleRepository;
        private readonly Repository<MatchedSample> _matchedSampleRepository;
        private readonly Repository<Mutation> _mutationRepository;
        private readonly Repository<MutationOccurrence> _mutationOccurrenceRepository;

        public MutationsDataService(UniteDbContext dbContext)
        {
            _dbContext = dbContext;

            _donorRepository = new DonorRepository(dbContext);
            _analysisRepository = new AnalysisRepository(dbContext);
            _sampleRepository = new SampleRepository(dbContext);
            _analysedSampleRepository = new AnalysedSampleRepository(dbContext);
            _matchedSampleRepository = new MatchedSampleRepository(dbContext);
            _mutationRepository = new MutationRepository(dbContext);
            _mutationOccurrenceRepository = new MutationOccurrenceRepository(dbContext);
        }

        public void SaveData(MutationsModel model, out MutationsUploadAudit audit)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                audit = new MutationsUploadAudit();

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

        public void SaveData(IEnumerable<MutationsModel> models, out MutationsUploadAudit audit)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                audit = new MutationsUploadAudit();

                foreach(var model in models)
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


        private void ProcessModel(MutationsModel model, ref MutationsUploadAudit audit)
        {
            var donorModel = model.Donor.ToEntity();
            var donor = GetOrCreate(donorModel, ref audit);

            var analysisModel = model.Analysis.ToEntity()
                .With(entity => entity.Donor, donor);
            var analysis = GetOrCreate(analysisModel, ref audit);

            foreach(var analysedSampleEntry in model.AnalysedSamples)
            {
                var sampleModel = analysedSampleEntry.ToEntity()
                    .With(entity => entity.Donor, donor);
                var sample = GetOrCreate(sampleModel, ref audit);

                var analysedSampleModel = new AnalysedSample()
                    .With(entity => entity.Analysis, analysis)
                    .With(entity => entity.Sample, sample);
                var analysedSample = GetOrCreate(analysedSampleModel, ref audit);

                if (analysedSampleEntry.MatchedSamples != null)
                {
                    foreach(var matchedSampleEntry in analysedSampleEntry.MatchedSamples)
                    {
                        var matchingSampleModel = matchedSampleEntry.ToEntity()
                            .With(entity => entity.Donor, donor);
                        var matchingSample = GetOrCreate(matchingSampleModel, ref audit);

                        var analysedMatchingSampleModel = new AnalysedSample()
                            .With(entity => entity.Analysis, analysis)
                            .With(entity => entity.Sample, matchingSample);
                        var analysedMatchingSample = GetOrCreate(analysedMatchingSampleModel, ref audit);

                        var matchedSampleModel = new MatchedSample()
                            .With(entity => entity.Analysed, analysedSample)
                            .With(entity => entity.Matched, analysedMatchingSample);
                        var matchedSample = GetOrCreate(matchedSampleModel, ref audit);
                    }
                }

                if (analysedSampleEntry.Mutations != null)
                {
                    foreach (var mutationEntry in analysedSampleEntry.Mutations)
                    {
                        var mutationModel = mutationEntry.ToEntity();
                        var mutation = GetOrCreate(mutationModel, ref audit);

                        var mutationOccurrenceModel = new MutationOccurrence()
                            .With(entity => entity.AnalysedSample, analysedSample)
                            .With(entity => entity.Mutation, mutation);
                        var mutationOccurrence = GetOrCreate(mutationOccurrenceModel, ref audit);

                        audit.Mutations.Add(mutation.Id);
                    }
                }
            }
        }


        private Donor GetOrCreate(in Donor model, ref MutationsUploadAudit audit)
        {
            var id = model.Id;

            var entity = _donorRepository.Entities
                .FirstOrDefault(entity =>
                    entity.Id == id
            );

            if (entity == null)
            {
                entity = _donorRepository.Add(model);

                audit.DonorsCreated++;
            }

            return entity;
        }

        private Analysis GetOrCreate(in Analysis model, ref MutationsUploadAudit audit)
        {
            var donorId = model.Donor.Id;
            var name = model.Name;

            var entity = _analysisRepository.Entities
                .Include(entity => entity.File)
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.Name == name
            );

            if (entity == null)
            {
                entity = _analysisRepository.Add(model);

                audit.AnalysesCreated++;
            }

            return entity;
        }

        private Sample GetOrCreate(in Sample model, ref MutationsUploadAudit audit)
        {
            var donorId = model.Donor.Id;
            var name = model.Name;
            var typeId = model.TypeId;

            var entity = _sampleRepository.Entities
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.Name == name &&
                    entity.TypeId == typeId
            );

            if (entity == null)
            {
                entity = _sampleRepository.Add(model);

                audit.SamplesCreated++;
            }

            return entity;
        }

        private AnalysedSample GetOrCreate(in AnalysedSample model, ref MutationsUploadAudit audit)
        {
            var analysisId = model.Analysis.Id;
            var sampleId = model.Sample.Id;

            var entity = _analysedSampleRepository.Entities
                .FirstOrDefault(entity =>
                    entity.AnalysisId == analysisId &&
                    entity.SampleId == sampleId
            );

            if (entity == null)
            {
                entity = _analysedSampleRepository.Add(model);

                audit.SamplesAnalysed++;
            }

            return entity;
        }

        private MatchedSample GetOrCreate(in MatchedSample model, ref MutationsUploadAudit audit)
        {
            var analysedSampleId = model.Analysed.Id;
            var matchedSampleId = model.Matched.Id;

            var entity = _matchedSampleRepository.Entities
                .FirstOrDefault(entity =>
                    entity.AnalysedSampleId == analysedSampleId &&
                    entity.MatchedSampleId == matchedSampleId
            );

            if (entity == null)
            {
                entity = _matchedSampleRepository.Add(model);

                audit.SamplesMatched++;
            }

            return entity;
        }

        private Mutation GetOrCreate(in Mutation model, ref MutationsUploadAudit audit)
        {
            var code = model.Code;

            var entity = _mutationRepository.Entities
                .FirstOrDefault(entity =>
                    entity.Code == code
            );

            if (entity == null)
            {
                entity = _mutationRepository.Add(model);

                audit.MutationsCreated++;
            }

            return entity;
        }

        private MutationOccurrence GetOrCreate(in MutationOccurrence model, ref MutationsUploadAudit audit)
        {
            var analysedSampleId = model.AnalysedSample.Id;
            var mutationId = model.Mutation.Id;

            var entity = _mutationOccurrenceRepository.Entities
                .FirstOrDefault(entity =>
                    entity.AnalysedSampleId == analysedSampleId &&
                    entity.MutationId == mutationId
            );

            if (entity == null)
            {
                entity = _mutationOccurrenceRepository.Add(model);

                audit.MutationsAssociated++;
            }

            return entity;
        }
    }
}
