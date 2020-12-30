using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class DonorRepository : Repository<Donor>
    {
        public DonorRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public Donor Find(string id)
        {
            var donor = Find(donor =>
                donor.Id == id);

            return donor;
        }

        protected override void Map(in Donor source, ref Donor target)
        {
            target.Id = source.Id;
        }
    }
}
