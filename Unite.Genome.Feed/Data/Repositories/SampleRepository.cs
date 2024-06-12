using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Specimens;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories.Specimens;

namespace Unite.Genome.Feed.Data.Repositories;

internal class SampleRepository
{
    private DomainDbContext _dbContext;
    private readonly SpecimenRepository _specimenRepository;


    public SampleRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _specimenRepository = new SpecimenRepository(dbContext);
    }


    public Sample FindOrCreate(SampleModel model)
    {
        if (model == null)
            return null;

        return Find(model) ?? Create(model);
    }

    public Sample Find(SampleModel model)
    {
        if (model.MatchedSample == null)
        {
            return _dbContext.Set<Sample>().FirstOrDefault(entity =>
                entity.Specimen.Donor.ReferenceId == model.Specimen.Donor.ReferenceId &&
                entity.Specimen.ReferenceId == model.Specimen.ReferenceId &&
                entity.Analysis.TypeId == model.Analysis.Type);
        }
        else
        {
            return _dbContext.Set<Sample>().FirstOrDefault(entity =>
                entity.Specimen.Donor.ReferenceId == model.Specimen.Donor.ReferenceId &&
                entity.Specimen.ReferenceId == model.Specimen.ReferenceId &&
                entity.Analysis.TypeId == model.Analysis.Type &&
                entity.MatchedSample.Specimen.Donor.ReferenceId == model.MatchedSample.Specimen.Donor.ReferenceId &&
                entity.MatchedSample.Specimen.ReferenceId == model.MatchedSample.Specimen.ReferenceId &&
                entity.MatchedSample.Analysis.TypeId == model.MatchedSample.Analysis.Type);
        }
    }

    public Sample Create(SampleModel model)
    {
        var entity = new Sample
        {
            Analysis = Create(model.Analysis),
            SpecimenId = FindOrCreate(model.Specimen).Id,
            MatchedSampleId = FindOrCreate(model.MatchedSample)?.Id,
            Purity = model.Purity,
            Ploidy = model.Ploidy,
            CellsNumber = model.CellsNumber,
            GenesModel = model.GenesModel
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }


    private Specimen FindOrCreate(SpecimenModel model)
    {
        if (model == null)
            return null;

        return _specimenRepository.FindOrCreate(model);
    }

    private Analysis Create(AnalysisModel analysisModel)
    {
        var entity = new Analysis
        {
            TypeId = analysisModel.Type,
            Date = analysisModel.Date,
            Day = analysisModel.Day,
            Parameters = analysisModel.Parameters
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }
}
