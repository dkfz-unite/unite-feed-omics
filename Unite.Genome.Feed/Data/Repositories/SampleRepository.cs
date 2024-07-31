using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Specimens;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories.Specimens;

namespace Unite.Genome.Feed.Data.Repositories;

public class SampleRepository
{
    private DomainDbContext _dbContext;
    private readonly AnalysisRepository _analysisRepository;
    private readonly SpecimenRepository _specimenRepository;
    

    public SampleRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _analysisRepository = new AnalysisRepository(dbContext);
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
        return _dbContext.Set<Sample>()
            .Include(entity => entity.Analysis)
            .FirstOrDefault(entity =>
                entity.Specimen.Donor.ReferenceId == model.Specimen.Donor.ReferenceId &&
                entity.Specimen.ReferenceId == model.Specimen.ReferenceId &&
                entity.Specimen.TypeId == model.Specimen.Type &&
                entity.Analysis.TypeId == model.Analysis.Type);
    }

    public Sample Create(SampleModel model)
    {
        var entity = new Sample
        {
            Analysis = _analysisRepository.Create(model.Analysis),
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

    public void Update(Sample entity, SampleModel model)
    {
        _analysisRepository.Update(entity.Analysis, model.Analysis);

        if (model.Purity != null)
            entity.Purity = model.Purity;

        if (model.Ploidy != null)
            entity.Ploidy = model.Ploidy;

        if (model.CellsNumber != null)
            entity.CellsNumber = model.CellsNumber;

        if (model.GenesModel != null)
            entity.GenesModel = model.GenesModel;

        _dbContext.Update(entity);
        _dbContext.SaveChanges();
    }


    private Specimen FindOrCreate(SpecimenModel model)
    {
        if (model == null)
            return null;

        return _specimenRepository.FindOrCreate(model);
    }
}
