using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class MatchedSampleRepository : Repository<MatchedSample>
    {
        public MatchedSampleRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in MatchedSample source, ref MatchedSample target)
        {
            target.AnalysedSampleId = source.Analysed?.Id ?? source.AnalysedSampleId;
            target.MatchedSampleId = source.Matched?.Id ?? source.MatchedSampleId;
        }
    }
}
