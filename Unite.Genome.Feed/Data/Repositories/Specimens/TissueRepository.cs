using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Data.Repositories.Specimens;

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
        var entity = _dbContext.Set<Specimen>()
            .Include(entity => entity.Tissue)
            .FirstOrDefault(entity =>
                entity.DonorId == donorId &&
                entity.Tissue.ReferenceId == referenceId
            );

        return entity;
    }

    public Specimen Create(int donorId, string referenceId)
    {
        var entity = new Specimen
        {
            DonorId = donorId,
            Tissue = new Tissue { ReferenceId = referenceId }
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
