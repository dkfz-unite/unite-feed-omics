using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Mutations.Feed.Mutations.Data.Models;

namespace Unite.Mutations.Feed.Mutations.Data.Repositories
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
            if (!string.IsNullOrWhiteSpace(donorModel.ReferenceId))
            {
                return Find(donorModel.ReferenceId);
            }

            return null;
        }

        public Donor Create(DonorModel donorModel)
        {
            var donor = new Donor();

            donor.ReferenceId = donorModel.ReferenceId;

            _dbContext.Donors.Add(donor);
            _dbContext.SaveChanges();

            return donor;
        }


        private Donor Find(string referenceId)
        {
            var donor = _dbContext.Donors.FirstOrDefault(donor =>
                donor.ReferenceId == referenceId
            );

            return donor;
        }
    }
}