using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Organoids;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Data.Mutations.Repositories
{
    internal class OrganoidRepository
    {
        private readonly DomainDbContext _dbContext;


        public OrganoidRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Specimen FindOrCreate(int donorId, string referenceId)
        {
            return Find(donorId, referenceId) ?? Create(donorId, referenceId);
        }

        public Specimen Find(int donorId, string referenceId)
        {
            var entity = _dbContext.Set<Specimen>()
                .Include(entity => entity.Organoid)
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.Organoid.ReferenceId == referenceId
                );

            return entity;
        }

        public Specimen Create(int donorId, string referenceId)
        {
            var entity = new Specimen
            {
                DonorId = donorId,
                Organoid = new Organoid { ReferenceId = referenceId }
            };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }
    }
}
