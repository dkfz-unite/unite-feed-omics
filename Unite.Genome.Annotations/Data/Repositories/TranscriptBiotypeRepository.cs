using Unite.Data.Entities.Genome;
using Unite.Data.Services;

namespace Unite.Genome.Annotations.Data.Repositories;

internal class TranscriptBiotypeRepository
{
    private readonly DomainDbContext _dbContext;


    public TranscriptBiotypeRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public TranscriptBiotype FindOrCreate(string value)
    {
        return Find(value) ?? Create(value);
    }

    public TranscriptBiotype Find(string value)
    {
        var entity = _dbContext.Set<TranscriptBiotype>()
            .FirstOrDefault(entity =>
                entity.Value == value
            );

        return entity;
    }

    public TranscriptBiotype Create(string value)
    {
        var entity = new TranscriptBiotype
        {
            Value = value
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
