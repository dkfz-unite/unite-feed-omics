using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class AnalysedSampleRepository
    {
        private DomainDbContext _dbContext;
        private readonly SampleRepository _sampleRepository;


        public AnalysedSampleRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _sampleRepository = new SampleRepository(dbContext);
        }


        public AnalysedSample FindOrCreate(int analysisId, AnalysedSampleModel analysedSampleModel)
        {
            return Find(analysisId, analysedSampleModel) ?? Create(analysisId, analysedSampleModel);
        }

        public AnalysedSample Find(int analysisId, AnalysedSampleModel analysedSampleModel)
        {
            if (analysedSampleModel.MatchedSample == null)
            {
                return Find(analysisId, analysedSampleModel.AnalysedSample);
            }
            else
            {
                return Find(analysisId, analysedSampleModel.AnalysedSample, analysedSampleModel.MatchedSample);
            }
        }

        public AnalysedSample Create(int analysisId, AnalysedSampleModel analysedSampleModel)
        {
            if (analysedSampleModel.MatchedSample == null)
            {
                return Create(analysisId, analysedSampleModel.AnalysedSample);
            }
            else
            {
                return Create(analysisId, analysedSampleModel.AnalysedSample, analysedSampleModel.MatchedSample);
            }
        }


        private AnalysedSample Find(int analysisId, SampleModel sampleModel)
        {
            var sample = _sampleRepository.Find(sampleModel);

            if (sample == null)
            {
                return null;
            }
            else
            {
                return _dbContext.AnalysedSamples.FirstOrDefault(analysedSample =>
                    analysedSample.AnalysisId == analysisId &&
                    analysedSample.SampleId == sample.Id &&
                    analysedSample.MatchedSampleId == null
                );
            }
        }

        private AnalysedSample Find(int analysisId, SampleModel sampleModel, SampleModel matchedSampleModel)
        {
            var sample = _sampleRepository.Find(sampleModel);
            var matchedSample = _sampleRepository.Find(matchedSampleModel);

            if (sample == null || matchedSample == null)
            {
                return null;
            }
            else
            {
                return _dbContext.AnalysedSamples.FirstOrDefault(analysedSample =>
                    analysedSample.AnalysisId == analysisId &&
                    analysedSample.SampleId == sample.Id &&
                    analysedSample.MatchedSampleId == matchedSample.Id
                );
            }
        }

        private AnalysedSample Create(int analysisId, SampleModel sampleModel)
        {
            var sample = _sampleRepository.FindOrCreate(sampleModel);

            var analysedSample = new AnalysedSample
            {
                AnalysisId = analysisId,
                SampleId = sample.Id,
                MatchedSampleId = null
            };

            _dbContext.AnalysedSamples.Add(analysedSample);
            _dbContext.SaveChanges();

            return analysedSample;
        }

        private AnalysedSample Create(int analysisId, SampleModel sampleModel, SampleModel matchedSampleModel)
        {
            var sample = _sampleRepository.FindOrCreate(sampleModel);
            var matchedSample = _sampleRepository.FindOrCreate(matchedSampleModel);

            var analysedSample = new AnalysedSample
            {
                AnalysisId = analysisId,
                SampleId = sample.Id,
                MatchedSampleId = matchedSample.Id
            };

            _dbContext.AnalysedSamples.Add(analysedSample);
            _dbContext.SaveChanges();

            return analysedSample;
        }
    }
}
