using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Organoids;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class OrganoidRepository
    {
        private readonly UniteDbContext _dbContext;


        public OrganoidRepository(UniteDbContext dbContext)
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
                .Include(specimen => specimen.Organoid)
                .FirstOrDefault(specimen =>
                    specimen.DonorId == donorId &&
                    specimen.Organoid.ReferenceId == referenceId
                );

            return specimen;
        }

        public Specimen Create(int donorId, string referenceId)
        {
            var specimen = new Specimen
            {
                DonorId = donorId,
                Organoid = new Organoid { ReferenceId = referenceId }
            };

            _dbContext.Specimens.Add(specimen);
            _dbContext.SaveChanges();

            return specimen;
        }
    }
}
