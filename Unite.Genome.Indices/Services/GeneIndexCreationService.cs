using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Genome.Indices.Services.Mappers;
using Unite.Indices.Entities.Genes;
using Unite.Indices.Services;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Genome.Indices.Services;

public class GeneIndexCreationService : IIndexCreationService<GeneIndex>
{
    private readonly DomainDbContext _dbContext;
    private readonly GeneIndexMapper _geneIndexMapper;
    private readonly VariantIndexMapper _variantIndexMapper;
    private readonly DonorIndexMapper _donorIndexMapper;
    private readonly ImageIndexMapper _imageIndexMapper;
    private readonly SpecimenIndexMapper _specimenIndexMapper;


    public GeneIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _geneIndexMapper = new GeneIndexMapper();
        _variantIndexMapper = new VariantIndexMapper();
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
    }


    public GeneIndex CreateIndex(object key)
    {
        var geneId = (int)key;

        return CreateGeneIndex(geneId);
    }


    private GeneIndex CreateGeneIndex(int geneId)
    {
        var gene = LoadGene(geneId);

        if (gene == null)
        {
            return null;
        }

        var index = CreateGeneIndex(gene);

        return index;
    }

    private GeneIndex CreateGeneIndex(Gene gene)
    {
        var index = new GeneIndex();

        _geneIndexMapper.Map(gene, index);

        index.Specimens = CreateSpecimenIndices(gene.Id);

        return index;
    }

    /// <summary>
    /// Loads specified gene.
    /// </summary>
    /// <param name="geneId">Gene identifier.</param>
    /// <returns>Gene.</returns>
    private Gene LoadGene(int geneId)
    {
        var gene = _dbContext.Set<Gene>()
            .Include(gene => gene.Info)
            .FirstOrDefault(gene => gene.Id == geneId);

        return gene;
    }


    private SpecimenIndex[] CreateSpecimenIndices(int geneId)
    {
        var specimens = LoadSpecimens(geneId);

        if (specimens == null)
        {
            return null;
        }

        var indices = specimens
            .Select(specimen => CreateSpecimenIndex(specimen, geneId))
            .ToArray();

        return indices;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, int geneId)
    {
        var index = new SpecimenIndex();

        index.Donor = CreateDonorIndex(specimen.Id, out var donor);

        index.Images = CreateImageIndices(specimen.Id, donor?.ClinicalData.DiagnosisDate);

        index.Variants = CreateVariantIndices(specimen.Id, geneId);

        _specimenIndexMapper.Map(specimen, index, donor?.ClinicalData.DiagnosisDate);

        return index;
    }

    /// <summary>
    /// Loads specimens having variants affecting specified gene.
    /// </summary>
    /// <param name="geneId">Gene identifier.</param>
    /// <returns>Array of specimens.</returns>
    private Specimen[] LoadSpecimens(int geneId)
    {
        var ssmAffectedSpecimenIds = _dbContext.Set<SSM.VariantOccurrence>()
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Select(occurrence => occurrence.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        var cnvAffectedSpecimenIds = _dbContext.Set<CNV.VariantOccurrence>()
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Select(occurrence => occurrence.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        var svAffectedSpecimenIds = _dbContext.Set<SV.VariantOccurrence>()
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Select(occurrence => occurrence.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        var specimenIds = ssmAffectedSpecimenIds.Union(cnvAffectedSpecimenIds).Union(svAffectedSpecimenIds).ToArray();

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

    /// <summary>
    /// Loads the donor of the specified specimen.
    /// </summary>
    /// <param name="specimenId">Specimen identifier.</param>
    /// <returns>Donor.</returns>
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

    /// <summary>
    /// Loads images of the specified specimen donor if the specimen is donor derived (e.g. tissue).
    /// </summary>
    /// <param name="specimenId">Specimen identifier.</param>
    /// <returns>Array of images.</returns>
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


    private VariantIndex[] CreateVariantIndices(int specimenId, int geneId)
    {
        var mutations = LoadMutations(specimenId, geneId);
        var copyNumberVariants = LoadCopyNumberVariants(specimenId, geneId);
        var structuralVariants = LoadStructuralVariants(specimenId, geneId);

        var indices = new List<VariantIndex>();

        if (mutations != null)
        {
            indices.AddRange(mutations.Select(variant => CreateVariantIndex(variant)));
        }

        if (copyNumberVariants != null)
        {
            indices.AddRange(copyNumberVariants.Select(variant => CreateVariantIndex(variant)));
        }

        if (structuralVariants != null)
        {
            indices.AddRange(structuralVariants.Select(variant => CreateVariantIndex(variant)));
        }

        return indices.Any() ? indices.ToArray() : null;
    }

    private VariantIndex CreateVariantIndex(SSM.Variant variant)
    {
        var index = new VariantIndex();

        _variantIndexMapper.Map(variant, index, false);

        return index;
    }

    private VariantIndex CreateVariantIndex(CNV.Variant variant)
    {
        var index = new VariantIndex();

        _variantIndexMapper.Map(variant, index, false);

        return index;
    }

    private VariantIndex CreateVariantIndex(SV.Variant variant)
    {
        var index = new VariantIndex();

        _variantIndexMapper.Map(variant, index, false);

        return index;
    }

    /// <summary>
    /// Loads all mutations affecting specified gene in specified specimen.
    /// </summary>
    /// <param name="specimenId">Specimen identifier.</param>
    /// <param name="geneId">Gene identifier.</param>
    /// <returns>Array of mutations.</returns>
    private SSM.Variant[] LoadMutations(int specimenId, int geneId)
    {
        // Variants should be filtered by the gene the're affecting.
        var variantIds = _dbContext.Set<SSM.VariantOccurrence>()
            .Where(occurrence => occurrence.AnalysedSample.Sample.SpecimenId == specimenId)
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().VariantId)
            .ToArray();

        var variants = _dbContext.Set<SSM.Variant>()
            .Include(variant => variant.AffectedTranscripts.Where(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Where(variant => variantIds.Contains(variant.Id))
            .ToArray();

        return variants;
    }

    /// <summary>
    /// Loads all copy number variants affecting specified gene in specified specimen.
    /// </summary>
    /// <param name="specimenId">Specimen identifier.</param>
    /// <param name="geneId">Gene identifier.</param>
    /// <returns>Array of copy number variants.</returns>
    private CNV.Variant[] LoadCopyNumberVariants(int specimenId, int geneId)
    {
        // Variants should be filtered by the gene the're affecting.
        var variantIds = _dbContext.Set<CNV.VariantOccurrence>()
            .Where(occurrence => occurrence.AnalysedSample.Sample.SpecimenId == specimenId)
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().VariantId)
            .ToArray();

        var variants = _dbContext.Set<CNV.Variant>()
            .Include(variant => variant.AffectedTranscripts.Where(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Where(variant => variantIds.Contains(variant.Id))
            .ToArray();

        return variants;
    }

    /// <summary>
    /// Loads all structural variants affecting specified gene in specified specimen.
    /// </summary>
    /// <param name="specimenId">Specimen identifier.</param>
    /// <param name="geneId">Gene identifier.</param>
    /// <returns>Array of structural variants.</returns>
    private SV.Variant[] LoadStructuralVariants(int specimenId, int geneId)
    {
        // Variants should be filtered by the gene the're affecting.
        var variantIds = _dbContext.Set<SV.VariantOccurrence>()
            .Where(occurrence => occurrence.AnalysedSample.Sample.SpecimenId == specimenId)
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().VariantId)
            .ToArray();

        var variants = _dbContext.Set<SV.Variant>()
            .Include(variant => variant.AffectedTranscripts.Where(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Where(variant => variantIds.Contains(variant.Id))
            .ToArray();

        return variants;
    }
}
