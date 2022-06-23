using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Mutations.Models;

namespace Unite.Genome.Feed.Data.Mutations.Repositories;

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
        var donor = new Donor();

        Map(model, ref donor);

        _dbContext.Add(donor);
        _dbContext.SaveChanges();

        return donor;
    }


    private void Map(in DonorModel model, ref Donor entity)
    {
        entity.ReferenceId = model.ReferenceId;
    }
}
