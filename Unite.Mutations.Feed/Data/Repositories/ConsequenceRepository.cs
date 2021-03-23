using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class ConsequenceRepository : Repository<Consequence>
    {
        public ConsequenceRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in Consequence source, ref Consequence target)
        {
            target.ImpactId = source.ImpactId;
            target.Severity = source.Severity;
        }
    }
}
