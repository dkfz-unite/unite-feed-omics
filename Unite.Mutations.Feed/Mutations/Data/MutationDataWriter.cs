using System.Collections.Generic;
using Unite.Data.Services;
using Unite.Mutations.Feed.Mutations.Data.Models;
using Unite.Mutations.Feed.Mutations.Data.Models.Audit;
using Unite.Mutations.Feed.Mutations.Data.Repositories;

namespace Unite.Mutations.Feed.Mutations.Data
{
    public class MutationDataWriter
    {
        private readonly UniteDbContext _dbContext;
        private readonly AnalysisRepository _analysisRepository;
        private readonly AnalysedSampleRepository _analysedSampleRepository;
        private readonly MatchedSampleRepository _matchedSampleRepository;
        private readonly MutationRepository _mutationRepository;
        private readonly MutationOccurrenceRepository _mutationOccurrenceRepository;

        public MutationDataWriter(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _analysisRepository = new AnalysisRepository(dbContext);
            _analysedSampleRepository = new AnalysedSampleRepository(dbContext);
            _matchedSampleRepository = new MatchedSampleRepository(dbContext);
            _mutationRepository = new MutationRepository(dbContext);
            _mutationOccurrenceRepository = new MutationOccurrenceRepository(dbContext);
        }


        public void SaveData(AnalysisModel model, out MutationsUploadAudit audit)
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

        public void SaveData(IEnumerable<AnalysisModel> models, out MutationsUploadAudit audit)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                audit = new MutationsUploadAudit();

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


        private void ProcessModel(AnalysisModel analysisModel, ref MutationsUploadAudit audit)
        {
            var analysis = _analysisRepository.FindOrCreate(analysisModel);

            foreach(var analysedSampleModel in analysisModel.AnalysedSamples)
            {
                var sample = _analysedSampleRepository.FindOrCreate(analysis.Id, analysedSampleModel);

                if(analysedSampleModel.MatchedSamples != null)
                {
                    foreach(var matchedSampleModel in analysedSampleModel.MatchedSamples)
                    {
                        var matchingSample = _analysedSampleRepository.FindOrCreate(analysis.Id, matchedSampleModel);

                        _matchedSampleRepository.FindOrCreate(sample.Id, matchingSample.Id);
                    }
                }

                if(analysedSampleModel.Mutations != null)
                {
                    var mutations = _mutationRepository.CreateMissing(analysedSampleModel.Mutations);

                    foreach(var mutation in mutations)
                    {
                        audit.Mutations.Add(mutation.Id);
                        audit.MutationsCreated++;
                    }

                    var mutationOccurrences = _mutationOccurrenceRepository.CreateMissing(sample.Id, analysedSampleModel.Mutations);

                    foreach(var mutationOccurrence in mutationOccurrences)
                    {
                        audit.MutationsAssociated++;
                    }
                }
            }
        }
    }
}
