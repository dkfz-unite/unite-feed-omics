using Unite.Data.Context;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Data.Entities.Specimens.Materials;

namespace Unite.Genome.Feed.Data.Repositories.Specimens;

internal class MaterialRepository
{
    private readonly DomainDbContext _dbContext;


    public MaterialRepository(DomainDbContext dbContext)
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
                entity.TypeId == SpecimenType.Material &&
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
            TypeId = SpecimenType.Material,
            Material = new Material { ReferenceId = referenceId }
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
