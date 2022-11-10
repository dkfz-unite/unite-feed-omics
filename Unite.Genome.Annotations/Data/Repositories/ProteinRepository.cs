using System.Linq.Expressions;
using Unite.Data.Entities.Genome;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models;

namespace Unite.Genome.Annotations.Data.Repositories;

internal class ProteinRepository
{
    private readonly DomainDbContext _dbContext;


    public ProteinRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Protein FindOrCreate(ProteinModel model, IEnumerable<Protein> cache = null)
    {
        return Find(model, cache) ?? Create(model);
    }

    public Protein Find(ProteinModel model, IEnumerable<Protein> cache = null)
    {
        Expression<Func<Protein, bool>> predicate = (entity) =>
            entity.Info.EnsemblId == model.EnsemblId;

        var entity = cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<Protein>().FirstOrDefault(predicate);

        return entity;
    }

    public Protein Create(ProteinModel model)
    {
        var protein = Convert(model);

        _dbContext.Add(protein);
        _dbContext.SaveChanges();

        return protein;
    }

    public IEnumerable<Protein> CreateMissing(IEnumerable<ProteinModel> models, IEnumerable<Protein> cache = null)
    {
        var entitiesToAdd = new List<Protein>();

        foreach (var model in models)
        {
            var entity = Find(model, cache);

            if (entity == null)
            {
                entity = Convert(model);

                entitiesToAdd.Add(entity);
            }
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }


    private Protein Convert(ProteinModel model)
    {
        var entity = new Protein
        {
            Start = model.Start,
            End = model.End,
            Length = model.Length,

            Info = new ProteinInfo
            {
                EnsemblId = model.EnsemblId
            }
        };

        return entity;
    }
}
