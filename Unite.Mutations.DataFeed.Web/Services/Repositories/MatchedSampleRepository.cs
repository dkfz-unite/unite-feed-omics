using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class MatchedSampleRepository : Repository<MatchedSample>
    {
        public MatchedSampleRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in MatchedSample source, ref MatchedSample target)
        {
            target.Analysed = source.Analysed;
            target.Matched = source.Matched;
        }
    }
}
