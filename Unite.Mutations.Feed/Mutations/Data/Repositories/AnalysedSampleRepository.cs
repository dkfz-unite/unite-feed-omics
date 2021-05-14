using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Mutations.Data.Models;

namespace Unite.Mutations.Feed.Mutations.Data.Repositories
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


        public AnalysedSample Find(int analysisId, SampleModel sampleModel)
        {
            var sample = _sampleRepository.Find(sampleModel);

            if(sample != null)
            {
                return Find(analysisId, sample.Id);
            }

            return null;
        }

        public AnalysedSample Create(int analysisId, SampleModel sampleModel)
        {
            var sample = _sampleRepository.FindOrCreate(sampleModel);

            return Create(analysisId, sample.Id);
        }

        public AnalysedSample FindOrCreate(int analysisId, SampleModel sampleModel)
        {
            return Find(analysisId, sampleModel) ?? Create(analysisId, sampleModel);
        }


        private AnalysedSample Find(int analysisId, int sampleId)
        {
            var analysedSample = _dbContext.AnalysedSamples.FirstOrDefault(analysedSample =>
                analysedSample.AnalysisId == analysisId &&
                analysedSample.SampleId == sampleId
            );

            return analysedSample;
        }

        private AnalysedSample Create(int analysisId, int sampleId)
        {
            var analysedSample = new AnalysedSample();

            analysedSample.AnalysisId = analysisId;
            analysedSample.SampleId = sampleId;

            _dbContext.AnalysedSamples.Add(analysedSample);
            _dbContext.SaveChanges();

            return analysedSample;
        }
    }
}
