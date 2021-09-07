using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Cells;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
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
            var specimen = _dbContext.Specimens
                .Include(specimen => specimen.CellLine)
                .FirstOrDefault(specimen =>
                    specimen.DonorId == donorId &&
                    specimen.CellLine.ReferenceId == referenceId
                );

            return specimen;
        }

        public Specimen Create(int donorId, string referenceId)
        {
            var specimen = new Specimen
            {
                DonorId = donorId,
                CellLine = new CellLine { ReferenceId = referenceId }
            };

            _dbContext.Specimens.Add(specimen);
            _dbContext.SaveChanges();

            return specimen;
        }
    }
}
