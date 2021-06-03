using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class MatchedSampleRepository
    {
        private readonly UniteDbContext _dbContext;


        public MatchedSampleRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public MatchedSample FindOrCreate(int analysedSampleId, int matchedSampleId)
        {
            return Find(analysedSampleId, matchedSampleId) ?? Create(analysedSampleId, matchedSampleId);
        }

        public MatchedSample Find(int analysedSampleId, int matchedSampleId)
        {
            var matchedSample = _dbContext.MatchedSamples.FirstOrDefault(matchedSample =>
                matchedSample.AnalysedSampleId == analysedSampleId &&
                matchedSample.MatchedSampleId == matchedSampleId
            );

            return matchedSample;
        }

        public MatchedSample Create(int analysedSampleId, int matchedSampleId)
        {
            var matchedSample = new MatchedSample
            {
                AnalysedSampleId = analysedSampleId,
                MatchedSampleId = matchedSampleId
            };

            _dbContext.MatchedSamples.Add(matchedSample);
            _dbContext.SaveChanges();

            return matchedSample;
        }
    }
}
