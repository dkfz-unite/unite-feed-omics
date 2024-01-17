using Unite.Data.Context;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Lines;
using Unite.Data.Entities.Specimens.Enums;

namespace Unite.Genome.Feed.Data.Repositories.Specimens;

public class LineRepository
{
    private readonly DomainDbContext _dbContext;


    public LineRepository(DomainDbContext dbContext)
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
                entity.TypeId == SpecimenType.Line &&
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
            TypeId = SpecimenType.Line,
            Line = new Line { ReferenceId = referenceId }
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
