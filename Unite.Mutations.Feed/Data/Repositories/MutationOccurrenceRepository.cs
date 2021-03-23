using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class MutationOccurrenceRepository : Repository<MutationOccurrence>
    {
        public MutationOccurrenceRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in MutationOccurrence source, ref MutationOccurrence target)
        {
            target.AnalysedSampleId = source.AnalysedSample?.Id ?? source.AnalysedSampleId;
            target.MutationId = source.Mutation?.Id ?? source.MutationId;
        }
    }
}
