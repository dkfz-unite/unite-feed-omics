using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class DonorRepository
    {
        private readonly UniteDbContext _dbContext;


        public DonorRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Donor FindOrCreate(DonorModel donorModel)
        {
            return Find(donorModel) ?? Create(donorModel);
        }

        public Donor Find(DonorModel donorModel)
        {
            var donor = _dbContext.Donors.FirstOrDefault(donor =>
                donor.ReferenceId == donorModel.ReferenceId
            );

            return donor;
        }

        public Donor Create(DonorModel donorModel)
        {
            var donor = new Donor();

            Map(donorModel, donor);

            _dbContext.Donors.Add(donor);
            _dbContext.SaveChanges();

            return donor;
        }


        private void Map(DonorModel donorModel, Donor donor)
        {
            donor.ReferenceId = donorModel.ReferenceId;
        }
    }
}