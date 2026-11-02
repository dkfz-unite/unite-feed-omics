using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis;
using Unite.Data.Entities.Specimens;
using Unite.Omics.Feed.Data.Models;
using Unite.Omics.Feed.Data.Repositories.Specimens;

namespace Unite.Omics.Feed.Data.Repositories;

public class SampleRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly AssayRepository _assayRepository;
    private readonly SpecimenRepository _specimenRepository;


    public SampleRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _assayRepository = new AssayRepository(dbContext);
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
                entity.Analysis.TypeId == model.Assay.Type);
    }

    public Sample Create(SampleModel model, SampleModel matchedSample = null)
    {
        var entity = new Sample
        {
            Analysis = _assayRepository.Create(model.Assay),
            SpecimenId = FindOrCreate(model.Specimen).Id,
            MatchedSampleId = FindOrCreate(matchedSample)?.Id,
            Genome = model.Genome,
            Purity = model.Purity,
            Ploidy = model.Ploidy,
            Cells = model.Cells,
            Batch = model.Batch
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public void Update(Sample entity, SampleModel model, SampleModel matchedSample = null)
    {
        _assayRepository.Update(entity.Analysis, model.Assay);

        if (matchedSample != null)
            entity.MatchedSampleId = FindOrCreate(matchedSample).Id;

        if (model.Genome != null)
            entity.Genome = model.Genome;

        if (model.Purity != null)
            entity.Purity = model.Purity;

        if (model.Ploidy != null)
            entity.Ploidy = model.Ploidy;

        if (model.Cells != null)
            entity.Cells = model.Cells;

        if (model.Batch != null)
            entity.Batch = model.Batch;

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
