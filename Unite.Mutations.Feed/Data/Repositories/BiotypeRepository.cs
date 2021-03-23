using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class BiotypeRepository : Repository<Biotype>
    {
        public BiotypeRepository(DbContext dbContext) : base(dbContext)
        {
        }

        protected override void Map(in Biotype source, ref Biotype target)
        {
            target.Value = source.Value;
        }
    }
}
