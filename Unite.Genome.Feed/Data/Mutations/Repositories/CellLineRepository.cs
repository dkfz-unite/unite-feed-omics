using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Cells;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Data.Mutations.Repositories
{
    public class CellLineRepository
    {
        private readonly DomainDbContext _dbContext;


        public CellLineRepository(DomainDbContext dbContext)
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
                .Include(entity => entity.CellLine)
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.CellLine.ReferenceId == referenceId
                );

            return entity;
        }

        public Specimen Create(int donorId, string referenceId)
        {
            var entity = new Specimen
            {
                DonorId = donorId,
                CellLine = new CellLine { ReferenceId = referenceId }
            };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }
    }
}
