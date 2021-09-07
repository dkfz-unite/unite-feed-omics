using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class TissueRepository
    {
        private readonly DomainDbContext _dbContext;


        public TissueRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Specimen FindOrCreate(int donorId, string referenceId)
        {
            return Find(donorId, referenceId) ?? Create(donorId, referenceId);
        }

        public Specimen Find(int donorId, string referenceId)
        {
            var specimen = _dbContext.Specimens
                .Include(specimen => specimen.Tissue)
                .FirstOrDefault(specimen =>
                    specimen.DonorId == donorId &&
                    specimen.Tissue.ReferenceId == referenceId
                );

            return specimen;
        }

        public Specimen Create(int donorId, string referenceId)
        {
            var specimen = new Specimen
            {
                DonorId = donorId,
                Tissue = new Tissue { ReferenceId = referenceId }
            };

            _dbContext.Specimens.Add(specimen);
            _dbContext.SaveChanges();

            return specimen;
        }
    }
}
