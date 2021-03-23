using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class AnalysedSampleRepository : Repository<AnalysedSample>
    {
        public AnalysedSampleRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in AnalysedSample source, ref AnalysedSample target)
        {
            target.AnalysisId = source.Analysis?.Id ?? source.AnalysisId;
            target.SampleId = source.Sample?.Id ?? source.SampleId;
        }
    }
}
