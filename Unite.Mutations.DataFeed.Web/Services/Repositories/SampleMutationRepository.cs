using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Samples;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class SampleMutationRepository : Repository<SampleMutation>
    {
        public SampleMutationRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public SampleMutation Find(int sampleId, int mutationId)
        {
            var sampleMutation = Find(sampleMutation =>
                sampleMutation.SampleId == sampleId &&
                sampleMutation.MutationId == mutationId);

            return sampleMutation;
        }

        protected override void Map(in SampleMutation source, ref SampleMutation target)
        {
            target.Sample = source.Sample;
            target.Mutation = source.Mutation;
            target.Quality = source.Quality;
            target.Filter = source.Filter;
            target.Info = source.Info;
        }
    }
}
