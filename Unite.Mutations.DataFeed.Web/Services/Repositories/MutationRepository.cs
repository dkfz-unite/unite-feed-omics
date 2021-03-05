using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class MutationRepository : Repository<Mutation>
    {
        public MutationRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public Mutation Find(string code)
        {
            var mutation = Find(mutation =>
                mutation.Code == code);

            return mutation;
        }

        protected override void Map(in Mutation source, ref Mutation target)
        {
            target.Code = source.Code;
            target.ChromosomeId = source.ChromosomeId;
            target.SequenceTypeId = source.SequenceTypeId;
            target.Start = source.Start;
            target.End = source.End;
            target.TypeId = source.TypeId;
            target.ReferenceBase = source.ReferenceBase;
            target.AlternateBase = source.AlternateBase;
        }
    }
}
