using System.Linq.Expressions;
using Unite.Data.Entities.Genome;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models;

namespace Unite.Genome.Annotations.Data.Repositories;

internal class GeneRepository
{
    private readonly DomainDbContext _dbContext;


    public GeneRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Gene FindOrCreate(GeneModel model, IEnumerable<Gene> cache = null)
    {
        return Find(model, cache) ?? Create(model);
    }

    public Gene Find(GeneModel model, IEnumerable<Gene> cache = null)
    {
        Expression<Func<Gene, bool>> predicate = (entity) =>
            entity.Info.EnsemblId == model.EnsemblId;

        var entity = cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<Gene>().FirstOrDefault(predicate);

        return entity;
    }

    public Gene Create(GeneModel model)
    {
        var gene = Convert(model);

        _dbContext.Add(gene);
        _dbContext.SaveChanges();

        return gene;
    }

    public IEnumerable<Gene> CreateMissing(IEnumerable<GeneModel> models, IEnumerable<Gene> cache = null)
    {
        var entitiesToAdd = new List<Gene>();

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


    private Gene Convert(GeneModel model)
    {
        var entity = new Gene
        {
            Symbol = model.Symbol,
            ChromosomeId = model.Chromosome,
            Start = model.Start,
            End = model.End,
            Strand = model.Strand,
            Biotype = model.Biotype,

            Info = new GeneInfo
            {
                EnsemblId = model.EnsemblId
            }
        };

        return entity;
    }
}
