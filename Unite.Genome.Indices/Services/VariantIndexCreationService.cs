using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
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


    public VariantIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _variantIndexMapper = new VariantIndexMapper();
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
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

        index.Specimens = CreateSpecimenIndices(variant.Id);

        index.NumberOfDonors = index.Specimens
            .DistinctBy(specimen => specimen.Donor.Id)
            .Count();

        index.NumberOfSpecimens = index.Specimens
            .DistinctBy(specimen => specimen.Id)
            .Count();

        index.NumberOfGenes += index.AffectedTranscripts?
            .Where(affectedTranscript => affectedTranscript.Gene != null)
            .DistinctBy(affectedTranscript => affectedTranscript.Gene.Id)
            .Count() ?? 0;


        return index;
    }

    private TVariant LoadVariant(long variantId)
    {
        var variant = _dbContext.Set<TVariant>()
            .IncludeAffectedTranscripts()
            .FirstOrDefault(variant => variant.Id == variantId);

        return variant;
    }


    private SpecimenIndex[] CreateSpecimenIndices(long variantId)
    {
        var specimens = LoadSpecimens(variantId);

        if (specimens == null)
        {
            return null;
        }

        var indices = specimens
            .Select(specimen => CreateSpecimenIndex(specimen))
            .ToArray();

        return indices;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen)
    {
        var index = new SpecimenIndex();

        index.Donor = CreateDonorIndex(specimen.Id, out var donor);

        index.Images = CreateImageIndices(specimen.Id, donor.ClinicalData?.DiagnosisDate);

        _specimenIndexMapper.Map(specimen, index, donor.ClinicalData?.DiagnosisDate);

        return index;
    }

    private Specimen[] LoadSpecimens(long variantId)
    {
        var specimenIds = _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => occurrence.VariantId == variantId)
            .Select(occurrence => occurrence.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        var specimens = _dbContext.Set<Specimen>()
            .IncludeTissue()
            .IncludeCellLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArray();

        return specimens;
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
            .Include(specimen => specimen.Tissue)
            .Where(specimen => specimen.Tissue != null)
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
