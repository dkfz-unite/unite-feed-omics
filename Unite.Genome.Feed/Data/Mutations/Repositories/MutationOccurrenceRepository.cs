using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Mutations.Models;

namespace Unite.Genome.Feed.Data.Mutations.Repositories;

internal class MutationOccurrenceRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly MutationRepository _mutationRepository;


    public MutationOccurrenceRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _mutationRepository = new MutationRepository(dbContext);
    }


    public MutationOccurrence FindOrCreate(int analysedSampleId, MutationModel model)
    {
        return Find(analysedSampleId, model) ?? Create(analysedSampleId, model);
    }

    public MutationOccurrence Find(int analysedSampleId, MutationModel model)
    {
        var mutation = _mutationRepository.Find(model);

        if (mutation != null)
        {
            return Find(analysedSampleId, mutation.Id);
        }

        return null;
    }

    public MutationOccurrence Create(int analysedSampleId, MutationModel model)
    {
        var mutation = _mutationRepository.FindOrCreate(model);

        return Create(analysedSampleId, mutation.Id);
    }

    public IEnumerable<MutationOccurrence> CreateOrUpdate(int analysedSampleId, IEnumerable<MutationModel> models)
    {
        RemoveRedundant(analysedSampleId, models);

        var created = CreateMissing(analysedSampleId, models);

        return created;
    }

    public IEnumerable<MutationOccurrence> CreateMissing(int analysedSampleId, IEnumerable<MutationModel> models)
    {
        var entitiesToAdd = new List<MutationOccurrence>();

        foreach (var model in models)
        {
            var mutation = _mutationRepository.FindOrCreate(model);

            var entity = Find(analysedSampleId, mutation.Id);

            if (entity == null)
            {
                entity = Convert(analysedSampleId, mutation.Id);

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

    public void RemoveRedundant(int analysedSampleId, IEnumerable<MutationModel> models)
    {
        var mutationCodes = models.Select(model => model.Code);

        var entitiesToRemove = _dbContext.Set<MutationOccurrence>()
            .Where(entity => entity.AnalysedSampleId == analysedSampleId && !mutationCodes.Contains(entity.Mutation.Code))
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private MutationOccurrence Find(int analysedSampleId, long mutationId)
    {
        var entity = _dbContext.Set<MutationOccurrence>()
            .FirstOrDefault(entity =>
                entity.AnalysedSampleId == analysedSampleId &&
                entity.MutationId == mutationId
            );

        return entity;
    }

    private MutationOccurrence Create(int analysedSampleId, long mutationId)
    {
        var entity = Convert(analysedSampleId, mutationId);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private MutationOccurrence Convert(int analysedSampleId, long mutationId)
    {
        var entity = new MutationOccurrence
        {
            AnalysedSampleId = analysedSampleId,
            MutationId = mutationId
        };

        return entity;
    }
}
