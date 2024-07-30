using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Data.Repositories;

internal class AnalysisRepository
{
    private readonly DomainDbContext _dbContext;


    public AnalysisRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Analysis Create(AnalysisModel model)
    {
        var entity = new Analysis
        {
            TypeId = model.Type,
            Date = model.Date,
            Day = model.Day,
            Parameters = model.Parameters
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public void Update(Analysis entity, AnalysisModel model)
    {
        if (entity.Parameters.IsNotEmpty())
        {
            _dbContext.RemoveRange(entity.Parameters);
            _dbContext.SaveChanges();
        }

        entity.TypeId = model.Type;
        entity.Date = model.Date;
        entity.Day = model.Day;
        entity.Parameters = model.Parameters;
        
        _dbContext.SaveChanges();
    }
}
