using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Genome.Indices.Services.Mappers;
using Unite.Indices.Entities.Variants;
using Unite.Indices.Services;

namespace Unite.Genome.Indices.Services;

public class VariantIndexCreationService<TVariant, TVariantOccurrence> : IIndexCreationService<VariantIndex>
    where TVariant : Data.Entities.Genome.Variants.Variant
    where TVariantOccurrence : Data.Entities.Genome.Variants.VariantOccurrence<TVariant>
{
    private readonly DomainDbContext _dbContext;
    private readonly VariantIndexMapper _variantIndexMapper;
    private readonly DonorIndexMapper _donorIndexMapper;
    private readonly ImageIndexMapper _imageIndexMapper;
    private readonly SpecimenIndexMapper _specimenIndexMapper;
    private readonly SampleIndexMapper _sampleIndexMapper;


    public VariantIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _variantIndexMapper = new VariantIndexMapper();
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
        _sampleIndexMapper = new SampleIndexMapper();
    }


    public VariantIndex CreateIndex(object key)
    {
        var variantId = (long)key;

        return CreateVariantIndex(variantId);
    }


    private VariantIndex CreateVariantIndex(long variantId)
    {
        var variant = LoadVariant(variantId);

        var index = CreateVariantIndex(variant);

        return index;
    }

    private VariantIndex CreateVariantIndex(TVariant variant)
    {
        if (variant == null)
        {
            return null;
        }

        var index = new VariantIndex();

        _variantIndexMapper.Map(variant, index);

        index.Samples = CreateSampleIndices(variant.Id);

        return index;
    }

    private TVariant LoadVariant(long variantId)
    {
        var variant = _dbContext.Set<TVariant>()
            .IncludeAffectedTranscripts()
            .FirstOrDefault(variant => variant.Id == variantId);

        return variant;
    }


    private SampleIndex[] CreateSampleIndices(long variantId)
    {
        var samples = LoadSamples(variantId);

        if (samples == null)
        {
            return null;
        }

        var indices = samples
            .Select(sample => CreateSampleIndex(sample.Sample, sample.Analyses))
            .ToArray();
        
        return indices;
    }

    private SampleIndex CreateSampleIndex(Sample sample, Analysis[] analyses)
    {
        var index = new SampleIndex();

        index.Donor = CreateDonorIndex(sample.SpecimenId, out var donor);

        index.Specimen = CreateSpecimenIndex(sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        index.Images = CreateImageIndices(sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        _sampleIndexMapper.Map(sample, analyses, index, donor.ClinicalData?.DiagnosisDate);

        return index;
    }

    private (Sample Sample, Analysis[] Analyses)[] LoadSamples(long variantId)
    {
        var analysedSampleIds = _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => occurrence.VariantId == variantId)
            .Select(occurrence => occurrence.AnalysedSampleId)
            .Distinct()
            .ToArray();

        var analysedSamples = _dbContext.Set<AnalysedSample>()
            .Include(analysedSample => analysedSample.Sample)
            .Include(analysedSample => analysedSample.Analysis)
            .Where(analysedSample => analysedSampleIds.Contains(analysedSample.Id))
            .ToArray();

        var samples = analysedSamples
            .GroupBy(analysedSample => analysedSample.SampleId)
            .Select(group => (group.First().Sample, group.Select(sample => sample.Analysis).ToArray()))
            .ToArray();

        return samples;
    }


    private SpecimenIndex CreateSpecimenIndex(int specimenId, DateOnly? diagnosisDate)
    {
        var specimen = LoadSpecimen(specimenId);

        if (specimen == null)
        {
            return null;
        }

        var index = CreateSpecimenIndex(specimen, diagnosisDate);

        return index;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? diagnosisDate)
    {
        var index = new SpecimenIndex();

        _specimenIndexMapper.Map(specimen, index, diagnosisDate);

        return index;
    }

    private Specimen LoadSpecimen(int specimenId)
    {
        var specimen = _dbContext.Set<Specimen>()
            .IncludeTissue()
            .IncludeCellLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .FirstOrDefault(specimen => specimen.Id == specimenId);

        return specimen;
    }


    private DonorIndex CreateDonorIndex(int specimenId, out Donor donor)
    {
        donor = LoadDonor(specimenId);

        if (donor == null)
        {
            return null;
        }

        var index = CreateDonorIndex(donor);

        return index;
    }

    private DonorIndex CreateDonorIndex(Donor donor)
    {
        var index = new DonorIndex();

        _donorIndexMapper.Map(donor, index);

        return index;
    }

    private Donor LoadDonor(int specimenId)
    {
        var donorId = _dbContext.Set<Specimen>()
            .Where(specimen => specimen.Id == specimenId)
            .Select(specimen => specimen.DonorId)
            .FirstOrDefault();

        var donor = _dbContext.Set<Donor>()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeProjects()
            .IncludeStudies()
            .Where(donor => donor.Id == donorId)
            .FirstOrDefault();

        return donor;
    }


    private ImageIndex[] CreateImageIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(specimenId);

        if (images == null)
        {
            return null;
        }

        var indices = images
            .Select(image => CreateImageIndex(image, diagnosisDate))
            .ToArray();

        return indices;
    }

    private ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        var index = new ImageIndex();

        _imageIndexMapper.Map(image, index, diagnosisDate);

        return index;
    }

    private Image[] LoadImages(int specimenId)
    {
        var donorId = _dbContext.Set<Specimen>()
            .Where(specimen => specimen.Tissue.TypeId == TissueType.Tumor)
            .Where(specimen => specimen.Id == specimenId)
            .Select(specimen => specimen.DonorId)
            .FirstOrDefault();

        var images = _dbContext.Set<Image>()
            .Include(image => image.MriImage)
            .Where(image => image.DonorId == donorId)
            .ToArray();

        return images;
    }
}
