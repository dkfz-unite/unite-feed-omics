using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Xenografts;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    public class XenograftRepository
    {
        private readonly UniteDbContext _dbContext;


        public XenograftRepository(UniteDbContext dbContext)
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
                .Include(specimen => specimen.Xenograft)
                .FirstOrDefault(specimen =>
                    specimen.DonorId == donorId &&
                    specimen.Xenograft.ReferenceId == referenceId
                );

            return specimen;
        }

        public Specimen Create(int donorId, string referenceId)
        {
            var specimen = new Specimen
            {
                DonorId = donorId,
                Xenograft = new Xenograft { ReferenceId = referenceId }
            };

            _dbContext.Specimens.Add(specimen);
            _dbContext.SaveChanges();

            return specimen;
        }
    }
}
