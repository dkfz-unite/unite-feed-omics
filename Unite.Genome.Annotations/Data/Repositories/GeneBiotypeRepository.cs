using Unite.Data.Entities.Genome;
using Unite.Data.Services;

namespace Unite.Genome.Annotations.Data.Repositories;

internal class GeneBiotypeRepository
{
    private readonly DomainDbContext _dbContext;


    public GeneBiotypeRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public GeneBiotype FindOrCreate(string value)
    {
        return Find(value) ?? Create(value);
    }

    public GeneBiotype Find(string value)
    {
        var entity = _dbContext.Set<GeneBiotype>()
            .FirstOrDefault(entity =>
                entity.Value == value
            );

        return entity;
    }

    public GeneBiotype Create(string value)
    {
        var entity = new GeneBiotype
        {
            Value = value
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
