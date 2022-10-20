using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories.Specimens;

namespace Unite.Genome.Feed.Data.Repositories;

internal class SampleRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly SpecimenRepository _specimenRepository;


    public SampleRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _specimenRepository = new SpecimenRepository(dbContext);
    }


    public Sample FindOrCreate(SampleModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Sample Find(SampleModel model)
    {
        var specimen = _specimenRepository.Find(model.Specimen);

        if (specimen == null)
        {
            return null;
        }

        var entity = _dbContext.Set<Sample>()
            .FirstOrDefault(entity =>
                entity.SpecimenId == specimen.Id &&
                entity.ReferenceId == model.ReferenceId
            );

        return entity;
    }

    public Sample Create(SampleModel model)
    {
        var specimen = _specimenRepository.FindOrCreate(model.Specimen);

        var entity = new Sample
        {
            SpecimenId = specimen.Id,
            ReferenceId = model.ReferenceId
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
