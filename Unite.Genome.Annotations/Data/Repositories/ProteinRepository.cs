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


    public Protein FindOrCreate(ProteinModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Protein Find(ProteinModel proteinModel)
    {
        var entity = _dbContext.Set<Protein>()
            .FirstOrDefault(entity =>
                entity.Info.EnsemblId == proteinModel.EnsemblId
            );

        return entity;
    }

    public Protein Create(ProteinModel model)
    {
        var protein = Convert(model);

        _dbContext.Add(protein);
        _dbContext.SaveChanges();

        return protein;
    }

    public IEnumerable<Protein> CreateMissing(IEnumerable<ProteinModel> models)
    {
        var entitiesToAdd = new List<Protein>();

        foreach (var model in models)
        {
            var entity = Find(model);

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
