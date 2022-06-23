using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models;

namespace Unite.Genome.Annotations.Data.Repositories;

internal class MutationRepository
{
    private readonly DomainDbContext _dbContext;


    public MutationRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Mutation Find(MutationModel model)
    {
        var entity = _dbContext.Set<Mutation>()
            .FirstOrDefault(entity =>
                entity.Code == model.Code
            );

        return entity;
    }
}
