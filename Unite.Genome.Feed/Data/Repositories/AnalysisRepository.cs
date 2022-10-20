using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Data.Repositories;

internal class AnalysisRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly SampleRepository _sampleRepository;


    public AnalysisRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _sampleRepository = new SampleRepository(dbContext);
    }


    public Analysis FindOrCreate(AnalysisModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Analysis Find(AnalysisModel model)
    {
        var query = _dbContext.Set<Analysis>().AsQueryable();

        // Analysis type should match
        query = query.Where(analysis =>
            analysis.TypeId == model.Type
        );

        // Analysis date should match
        query = query.Where(analysis =>
            analysis.Date == model.Date
        );

        // Number of analysed samples should match
        query = query.Where(analysis =>
            analysis.AnalysedSamples.Count() == model.AnalysedSamples.Count()
        );

        // Each analysed sample should match
        foreach (var analysedSampleModel in model.AnalysedSamples)
        {
            if (analysedSampleModel.MatchedSample == null)
            {
                // Analysed sample should match
                var sample = _sampleRepository.Find(analysedSampleModel.AnalysedSample);

                if (sample == null)
                {
                    return null;
                }
                else
                {
                    query = query.Where(analysis =>
                        analysis.AnalysedSamples.Any(analysedSample =>
                            analysedSample.SampleId == sample.Id &&
                            analysedSample.MatchedSampleId == null
                        )
                    );
                }
            }
            else
            {
                // Analysed sample should match
                var sample = _sampleRepository.Find(analysedSampleModel.AnalysedSample);

                // Matched sample should match
                var matchedSample = _sampleRepository.Find(analysedSampleModel.MatchedSample);

                if (sample == null || matchedSample == null)
                {
                    return null;
                }
                else
                {
                    query = query.Where(analysis =>
                        analysis.AnalysedSamples.Any(analysedSample =>
                            analysedSample.SampleId == sample.Id &&
                            analysedSample.MatchedSampleId == matchedSample.Id
                        )
                    );
                }
            }
        }

        return query.FirstOrDefault();
    }

    public Analysis Create(AnalysisModel model)
    {
        var entity = new Analysis();

        Map(model, ref entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }


    private void Map(in AnalysisModel model, ref Analysis entity)
    {
        entity.TypeId = model.Type;
        entity.Date = model.Date;
        entity.Parameters = model.Parameters;
    }
}
