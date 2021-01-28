using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class AnalysedSampleRepository : Repository<AnalysedSample>
    {
        public AnalysedSampleRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in AnalysedSample source, ref AnalysedSample target)
        {
            target.Analysis = source.Analysis;
            target.Sample = source.Sample;
        }
    }
}
