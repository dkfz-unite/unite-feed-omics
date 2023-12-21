using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories.Specimens;

namespace Unite.Genome.Feed.Data.Repositories;

internal class AnalysisRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly SpecimenRepository _specimenRepository;


    public AnalysisRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _specimenRepository = new SpecimenRepository(dbContext);
    }


    public Analysis FindOrCreate(AnalysedSampleModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Analysis Find(AnalysedSampleModel model)
    {
        if (model.Analysis.ReferenceId != null)
        {
            return FindByReferenceId(model.Analysis.ReferenceId);
        }
        else
        {
            return FindByModel(model);
        }
    }

    private Analysis FindByReferenceId(string referenceId)
    {
        var query = _dbContext.Set<Analysis>().AsQueryable();

        // Reference ID should match
        query = query.Where(analysis =>
            analysis.ReferenceId == referenceId
        );

        return query.FirstOrDefault();
    }

    private Analysis FindByModel(AnalysedSampleModel model)
    {
        var query = _dbContext.Set<Analysis>().AsQueryable();

        // Analysis type should match
        query = query.Where(analysis =>
            analysis.TypeId == model.Analysis.Type
        );

        // Analysis date and day should match
        query = query.Where(analysis =>
            analysis.Date == model.Analysis.Date &&
            analysis.Day == model.Analysis.Day
        );

        if (model.MatchedSample == null)
        {
            // Target sample should match
            var targetSample = _specimenRepository.Find(model.TargetSample);

            if (targetSample != null)
            {
                query = query.Where(analysis => analysis.AnalysedSample.TargetSampleId == targetSample.Id);
            }
            else
            {
                return null;
            }
        }
        else
        {
            // Target sample should match
            var targetSample = _specimenRepository.Find(model.TargetSample);

            // Matched sample should match
            var matchedSample = _specimenRepository.Find(model.MatchedSample);

            if (targetSample == null || matchedSample == null)
            {
                return null;
            }
            else
            {
                query = query.Where(analysis =>
                    analysis.AnalysedSample.TargetSampleId == targetSample.Id &&
                    analysis.AnalysedSample.MatchedSampleId == matchedSample.Id
                );
            }
        }

        return query.FirstOrDefault();
    }

    public Analysis Create(AnalysedSampleModel model)
    {
        var entity = new Analysis() { ReferenceId = model.Analysis.ReferenceId };

        Map(model.Analysis, ref entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }


    private static void Map(in AnalysisModel model, ref Analysis entity)
    {
        entity.TypeId = model.Type;
        entity.Date = model.Date;
        entity.Day = model.Day;
        entity.Parameters = model.Parameters;
    }
}
