using Unite.Data.Context;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Data.Entities.Specimens.Xenografts;

namespace Unite.Omics.Feed.Data.Repositories.Specimens;

public class XenograftRepository
{
    private readonly DomainDbContext _dbContext;


    public XenograftRepository(DomainDbContext dbContext)
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
            .FirstOrDefault(entity =>
                entity.DonorId == donorId &&
                entity.TypeId == SpecimenType.Xenograft &&
                entity.ReferenceId == referenceId
            );

        return entity;
    }

    public Specimen Create(int donorId, string referenceId)
    {
        var entity = new Specimen
        {
            DonorId = donorId,
            ReferenceId = referenceId,
            TypeId = SpecimenType.Xenograft,
            Xenograft = new Xenograft()
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
