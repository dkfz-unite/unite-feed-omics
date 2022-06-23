using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Mutations.Models;

namespace Unite.Genome.Feed.Data.Mutations.Repositories;

internal class AnalysedSampleRepository
{
    private DomainDbContext _dbContext;
    private readonly SampleRepository _sampleRepository;


    public AnalysedSampleRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _sampleRepository = new SampleRepository(dbContext);
    }


    public AnalysedSample FindOrCreate(int analysisId, AnalysedSampleModel model)
    {
        return Find(analysisId, model) ?? Create(analysisId, model);
    }

    public AnalysedSample Find(int analysisId, AnalysedSampleModel model)
    {
        if (model.MatchedSample == null)
        {
            return Find(analysisId, model.AnalysedSample);
        }
        else
        {
            return Find(analysisId, model.AnalysedSample, model.MatchedSample);
        }
    }

    public AnalysedSample Create(int analysisId, AnalysedSampleModel model)
    {
        if (model.MatchedSample == null)
        {
            return Create(analysisId, model.AnalysedSample);
        }
        else
        {
            return Create(analysisId, model.AnalysedSample, model.MatchedSample);
        }
    }


    private AnalysedSample Find(int analysisId, SampleModel sampleModel)
    {
        var sample = _sampleRepository.Find(sampleModel);

        if (sample == null)
        {
            return null;
        }
        else
        {
            return _dbContext.Set<AnalysedSample>().FirstOrDefault(entity =>
                entity.AnalysisId == analysisId &&
                entity.SampleId == sample.Id &&
                entity.MatchedSampleId == null
            );
        }
    }

    private AnalysedSample Find(int analysisId, SampleModel sampleModel, SampleModel matchedSampleModel)
    {
        var sample = _sampleRepository.Find(sampleModel);
        var matchedSample = _sampleRepository.Find(matchedSampleModel);

        if (sample == null || matchedSample == null)
        {
            return null;
        }
        else
        {
            return _dbContext.Set<AnalysedSample>().FirstOrDefault(entity =>
                entity.AnalysisId == analysisId &&
                entity.SampleId == sample.Id &&
                entity.MatchedSampleId == matchedSample.Id
            );
        }
    }

    private AnalysedSample Create(int analysisId, SampleModel sampleModel)
    {
        var sample = _sampleRepository.FindOrCreate(sampleModel);

        var entity = new AnalysedSample
        {
            AnalysisId = analysisId,
            SampleId = sample.Id,
            MatchedSampleId = null
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private AnalysedSample Create(int analysisId, SampleModel sampleModel, SampleModel matchedSampleModel)
    {
        var sample = _sampleRepository.FindOrCreate(sampleModel);
        var matchedSample = _sampleRepository.FindOrCreate(matchedSampleModel);

        var entity = new AnalysedSample
        {
            AnalysisId = analysisId,
            SampleId = sample.Id,
            MatchedSampleId = matchedSample.Id
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
