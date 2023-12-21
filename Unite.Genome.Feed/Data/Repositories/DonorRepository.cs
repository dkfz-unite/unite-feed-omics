using Unite.Data.Context;
using Unite.Data.Entities.Donors;
using Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Data.Repositories;

internal class DonorRepository
{
    private readonly DomainDbContext _dbContext;


    public DonorRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Donor FindOrCreate(DonorModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Donor Find(DonorModel model)
    {
        var entity = _dbContext.Set<Donor>()
            .FirstOrDefault(entity =>
                entity.ReferenceId == model.ReferenceId
            );

        return entity;
    }

    public Donor Create(DonorModel model)
    {
        var entity = new Donor { ReferenceId = model.ReferenceId };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
