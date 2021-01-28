using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class MutationOccurrenceRepository : Repository<MutationOccurrence>
    {
        public MutationOccurrenceRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in MutationOccurrence source, ref MutationOccurrence target)
        {
            target.AnalysedSample = source.AnalysedSample;
            target.Mutation = source.Mutation;
        }
    }
}
