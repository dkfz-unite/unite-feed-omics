using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class DonorRepository : Repository<Donor>
    {
        public DonorRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in Donor source, ref Donor target)
        {
            target.Id = source.Id;
        }
    }
}
