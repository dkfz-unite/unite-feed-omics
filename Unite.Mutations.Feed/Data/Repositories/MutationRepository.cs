using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class MutationRepository : Repository<Mutation>
    {
        public MutationRepository(DbContext database) : base(database)
        {
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
