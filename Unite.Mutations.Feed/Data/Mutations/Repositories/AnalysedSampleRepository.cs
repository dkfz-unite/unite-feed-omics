using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class AnalysedSampleRepository
    {
        private UniteDbContext _dbContext;
        private SampleRepository _sampleRepository;


        public AnalysedSampleRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _sampleRepository = new SampleRepository(dbContext);
        }


        public AnalysedSample FindOrCreate(int analysisId, SampleModel sampleModel)
        {
            return Find(analysisId, sampleModel) ?? Create(analysisId, sampleModel);
        }

        public AnalysedSample Find(int analysisId, SampleModel sampleModel)
        {
            var sample = _sampleRepository.Find(sampleModel);

            if (sample == null)
            {
                return null;
            }

            return Find(analysisId, sample.Id);
        }

        public AnalysedSample Create(int analysisId, SampleModel sampleModel)
        {
            var sample = _sampleRepository.FindOrCreate(sampleModel);

            var analysedSample = new AnalysedSample
            {
                AnalysisId = analysisId,
                SampleId = sample.Id
            };

            _dbContext.AnalysedSamples.Add(analysedSample);
            _dbContext.SaveChanges();

            return analysedSample;
        }


        private AnalysedSample Find(int analysisId, int sampleId)
        {
            var analysedSample = _dbContext.AnalysedSamples.FirstOrDefault(analysedSample =>
                analysedSample.AnalysisId == analysisId &&
                analysedSample.SampleId == sampleId
            );

            return analysedSample;
        }
    }
}
