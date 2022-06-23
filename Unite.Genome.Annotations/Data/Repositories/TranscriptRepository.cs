using Unite.Data.Entities.Genome;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models;

namespace Unite.Genome.Annotations.Data.Repositories;

public class TranscriptRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly TranscriptBiotypeRepository _biotypeRepository;
    private readonly GeneRepository _geneRepository;
    private readonly ProteinRepository _proteinRepository;


    public TranscriptRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _biotypeRepository = new TranscriptBiotypeRepository(dbContext);
        _geneRepository = new GeneRepository(dbContext);
        _proteinRepository = new ProteinRepository(dbContext);
    }


    public Transcript FindOrCreate(TranscriptModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Transcript Find(TranscriptModel model)
    {
        var entity = _dbContext.Set<Transcript>()
            .FirstOrDefault(entity =>
                entity.Info.EnsemblId == model.EnsemblId
            );

        return entity;
    }

    public Transcript Create(TranscriptModel model)
    {
        var entity = Convert(model);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public IEnumerable<Transcript> CreateMissing(IEnumerable<TranscriptModel> models)
    {
        var entitiesToAdd = new List<Transcript>();

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


    private Transcript Convert(TranscriptModel model)
    {
        var entity = new Transcript
        {
            Symbol = model.Symbol,
            ChromosomeId = model.Chromosome,
            Start = model.Start,
            End = model.End,
            Strand = model.Strand,

            Info = new TranscriptInfo
            {
                EnsemblId = model.EnsemblId
            }
        };

        if (model.Biotype != null)
        {
            entity.BiotypeId = _biotypeRepository.FindOrCreate(model.Biotype).Id;
        }

        if (model.Gene != null)
        {
            entity.GeneId = _geneRepository.FindOrCreate(model.Gene).Id;
        }

        if (model.Protein != null)
        {
            entity.ProteinId = _proteinRepository.FindOrCreate(model.Protein).Id;
        }

        return entity;
    }
}
