using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories.Specimens;

namespace Unite.Genome.Feed.Data.Repositories;

internal class AnalysedSampleRepository
{
    private DomainDbContext _dbContext;
    private readonly SpecimenRepository _specimenRepository;


    public AnalysedSampleRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _specimenRepository = new SpecimenRepository(dbContext);
    }


    public AnalysedSample FindOrCreate(int analysisId, AnalysedSampleModel model)
    {
        return Find(analysisId, model) ?? Create(analysisId, model);
    }

    public AnalysedSample Find(int analysisId, AnalysedSampleModel model)
    {
        if (model.MatchedSample == null)
        {
            return Find(analysisId, model.TargetSample);
        }
        else
        {
            return Find(analysisId, model.TargetSample, model.MatchedSample);
        }
    }

    public AnalysedSample Create(int analysisId, AnalysedSampleModel model)
    {
        if (model.MatchedSample == null)
        {
            return Create(analysisId, model, model.TargetSample);
        }
        else
        {
            return Create(analysisId, model, model.TargetSample, model.MatchedSample);
        }
    }


    private AnalysedSample Find(int analysisId, SpecimenModel targetSampleModel)
    {
        var targetSample = _specimenRepository.Find(targetSampleModel);

        if (targetSample == null)
        {
            return null;
        }
        else
        {
            return _dbContext.Set<AnalysedSample>().FirstOrDefault(entity =>
                entity.AnalysisId == analysisId &&
                entity.TargetSampleId == targetSample.Id &&
                entity.MatchedSampleId == null
            );
        }
    }

    private AnalysedSample Find(int analysisId, SpecimenModel targetSampleModel, SpecimenModel matchedSampleModel)
    {
        var targetSample = _specimenRepository.Find(targetSampleModel);
        var matchedSample = _specimenRepository.Find(matchedSampleModel);

        if (targetSample == null || matchedSample == null)
        {
            return null;
        }
        else
        {
            return _dbContext.Set<AnalysedSample>().FirstOrDefault(entity =>
                entity.AnalysisId == analysisId &&
                entity.TargetSampleId == targetSample.Id &&
                entity.MatchedSampleId == matchedSample.Id
            );
        }
    }

    private AnalysedSample Create(int analysisId, SampleModel analysedSampleModel, SpecimenModel targetSampleModel)
    {
        var targetSpecimen = _specimenRepository.FindOrCreate(targetSampleModel);

        var entity = new AnalysedSample
        {
            AnalysisId = analysisId,
            TargetSampleId = targetSpecimen.Id,
            MatchedSampleId = null,
            Ploidy = analysedSampleModel.Ploidy,
            Purity = analysedSampleModel.Purity
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private AnalysedSample Create(int analysisId, SampleModel analysedSampleModel, SpecimenModel targetSampleModel, SpecimenModel matchedSampleModel)
    {
        var targetSample = _specimenRepository.FindOrCreate(targetSampleModel);
        var matchedSample = _specimenRepository.FindOrCreate(matchedSampleModel);

        var entity = new AnalysedSample
        {
            AnalysisId = analysisId,
            TargetSampleId = targetSample.Id,
            MatchedSampleId = matchedSample.Id,
            Ploidy = analysedSampleModel.Ploidy,
            Purity = analysedSampleModel.Purity
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
