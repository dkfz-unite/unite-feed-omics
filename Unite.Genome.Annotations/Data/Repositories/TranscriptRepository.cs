using System.Linq.Expressions;
using Unite.Data.Entities.Genome;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models;

namespace Unite.Genome.Annotations.Data.Repositories;

public class TranscriptRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly GeneRepository _geneRepository;
    private readonly ProteinRepository _proteinRepository;


    public TranscriptRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _geneRepository = new GeneRepository(dbContext);
        _proteinRepository = new ProteinRepository(dbContext);
    }


    public Transcript FindOrCreate(
        TranscriptModel model,
        IEnumerable<Transcript> transcriptsCache = null,
        IEnumerable<Gene> genesCache = null,
        IEnumerable<Protein> proteinsCache = null)
    {
        return Find(model, transcriptsCache) ?? Create(model, genesCache, proteinsCache);
    }

    public Transcript Find(
        TranscriptModel model,
        IEnumerable<Transcript> cache = null)
    {
        Expression<Func<Transcript, bool>> predicate = (entity) =>
            entity.Info.EnsemblId == model.EnsemblId;

        var entity = cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<Transcript>().FirstOrDefault(predicate);

        return entity;
    }

    public Transcript Create(
        TranscriptModel model,
        IEnumerable<Gene> genesCache = null,
        IEnumerable<Protein> proteinsCache = null)
    {
        var entity = Convert(model, genesCache, proteinsCache);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public IEnumerable<Transcript> CreateMissing(
        IEnumerable<TranscriptModel> models,
        IEnumerable<Gene> genesCache = null,
        IEnumerable<Protein> proteinsCache = null)
    {
        var entitiesToAdd = new List<Transcript>();

        foreach (var model in models)
        {
            var entity = Find(model);

            if (entity == null)
            {
                entity = Convert(model, genesCache, proteinsCache);

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


    private Transcript Convert(
        TranscriptModel model,
        IEnumerable<Gene> genesCache = null,
        IEnumerable<Protein> proteinsCache = null)
    {
        var entity = new Transcript
        {
            Symbol = model.Symbol,
            ChromosomeId = model.Chromosome,
            Start = model.Start,
            End = model.End,
            Strand = model.Strand,
            Biotype = model.Biotype,

            Info = new TranscriptInfo
            {
                EnsemblId = model.EnsemblId
            }
        };

        if (model.Gene != null)
        {
            entity.GeneId = _geneRepository.FindOrCreate(model.Gene, genesCache).Id;
        }

        if (model.Protein != null)
        {
            entity.ProteinId = _proteinRepository.FindOrCreate(model.Protein, proteinsCache).Id;
        }

        return entity;
    }
}
