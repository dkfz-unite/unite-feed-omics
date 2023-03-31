using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Transcriptomics;
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
    private readonly GeneExpressionIndexMapper _geneExpressionIndexMapper;
    private readonly VariantIndexMapper _variantIndexMapper;
    private readonly SampleIndexMapper _sampleIndexMapper;
    private readonly DonorIndexMapper _donorIndexMapper;
    private readonly SpecimenIndexMapper _specimenIndexMapper;
    private readonly ImageIndexMapper _imageIndexMapper;


    public GeneIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _geneIndexMapper = new GeneIndexMapper();
        _geneExpressionIndexMapper = new GeneExpressionIndexMapper();
        _variantIndexMapper = new VariantIndexMapper();
        _sampleIndexMapper = new SampleIndexMapper();
        _donorIndexMapper = new DonorIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
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

        index.Samples = CreateSampleIndices(gene.Id);

        return index;
    }

    private Gene LoadGene(int geneId)
    {
        var gene = _dbContext.Set<Gene>()
            .AsNoTracking()
            .FirstOrDefault(gene => gene.Id == geneId);

        return gene;
    }


    private SampleIndex[] CreateSampleIndices(int geneId)
    {
        var samples = LoadSamples(geneId);

        if (samples == null)
        {
            return null;
        }

        var indices = samples
            .Select(sample => CreateSampleIndex(sample.Sample, sample.Analyses, geneId))
            .ToArray();

        return indices;
    }

    private SampleIndex CreateSampleIndex(Sample sample, Analysis[] analyses, int geneId)
    {
        var index = new SampleIndex();

        index.Donor = CreateDonorIndex(sample.SpecimenId, out var donor);

        index.Specimen = CreateSpecimenIndex(sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        index.Images = CreateImageIndices(sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        index.Variants = CreateVariantIndices(sample.Id, geneId);

        index.Expression = CreateExpressionIndex(sample.Id, geneId);

        _sampleIndexMapper.Map(sample, analyses, index, donor.ClinicalData?.DiagnosisDate);

        return index;
    }

    private (Sample Sample, Analysis[] Analyses)[] LoadSamples(int geneId)
    {
        var ssmAffectedSampleIds = _dbContext.Set<SSM.VariantOccurrence>()
            .AsNoTracking()
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Select(occurrence => occurrence.AnalysedSampleId)
            .Distinct()
            .ToArray();

        var cnvAffectedSampleIds = _dbContext.Set<CNV.VariantOccurrence>()
            .AsNoTracking()
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Select(occurrence => occurrence.AnalysedSampleId)
            .Distinct()
            .ToArray();

        var svAffectedSampleIds = _dbContext.Set<SV.VariantOccurrence>()
            .AsNoTracking()
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Select(occurrence => occurrence.AnalysedSampleId)
            .Distinct()
            .ToArray();

        // var exAffectedSampleIds = _dbContext.Set<GeneExpression>()
        //     .AsNoTracking()
        //     .Where(expression => expression.GeneId == geneId)
        //     .Select(expression => expression.AnalysedSampleId)
        //     .Distinct()
        //     .ToArray();

        var analysedSampleIds = Enumerable.Empty<int>()
            .Union(ssmAffectedSampleIds)
            .Union(cnvAffectedSampleIds)
            .Union(svAffectedSampleIds)
            // .Union(exAffectedSampleIds)
            .ToArray();

        var analysedSamples = _dbContext.Set<AnalysedSample>()
            .AsNoTracking()
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
            .AsNoTracking()
            .Where(specimen => specimen.Id == specimenId)
            .Select(specimen => specimen.DonorId)
            .FirstOrDefault();

        var donor = _dbContext.Set<Donor>()
            .AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeProjects()
            .IncludeStudies()
            .Where(donor => donor.Id == donorId)
            .FirstOrDefault();

        return donor;
    }


    private SpecimenIndex CreateSpecimenIndex(int specimenId, DateOnly? diagnosisDate)
    {
        var specimen = LoadSpecimen(specimenId);

        if (specimen == null)
        {
            return null;
        }

        var indices = CreateSpecimenIndex(specimen, diagnosisDate);

        return indices;
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
            .AsNoTracking()
            .IncludeTissue()
            .IncludeCellLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .FirstOrDefault(specimen => specimen.Id == specimenId);

        return specimen;
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
            .AsNoTracking()
            .Where(specimen => specimen.Tissue.TypeId == TissueType.Tumor)
            .Where(specimen => specimen.Id == specimenId)
            .Select(specimen => specimen.DonorId)
            .FirstOrDefault();

        var images = _dbContext.Set<Image>()
            .AsNoTracking()
            .Include(image => image.MriImage)
            .Where(image => image.DonorId == donorId)
            .ToArray();

        return images;
    }


    private VariantIndex[] CreateVariantIndices(int sampleId, int geneId)
    {
        var mutations = LoadMutations(sampleId, geneId);
        var copyNumberVariants = LoadCopyNumberVariants(sampleId, geneId);
        var structuralVariants = LoadStructuralVariants(sampleId, geneId);

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

        _variantIndexMapper.Map(variant, index);

        return index;
    }

    private VariantIndex CreateVariantIndex(CNV.Variant variant)
    {
        var index = new VariantIndex();

        _variantIndexMapper.Map(variant, index);

        return index;
    }

    private VariantIndex CreateVariantIndex(SV.Variant variant)
    {
        var index = new VariantIndex();

        _variantIndexMapper.Map(variant, index);

        return index;
    }

    private SSM.Variant[] LoadMutations(int sampleId, int geneId)
    {
        // Variants should be filtered by the gene the're affecting.
        var variantIds = _dbContext.Set<SSM.VariantOccurrence>()
            .AsNoTracking()
            .Where(occurrence => occurrence.AnalysedSample.SampleId == sampleId)
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().VariantId)
            .ToArray();

        var variants = _dbContext.Set<SSM.Variant>()
            .AsNoTracking()
            .Include(variant => variant.AffectedTranscripts.Where(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Where(variant => variantIds.Contains(variant.Id))
            .ToArray();

        return variants;
    }

    private CNV.Variant[] LoadCopyNumberVariants(int sampleId, int geneId)
    {
        // Variants should be filtered by the gene the're affecting.
        var variantIds = _dbContext.Set<CNV.VariantOccurrence>()
            .AsNoTracking()
            .Where(occurrence => occurrence.AnalysedSample.SampleId == sampleId)
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().VariantId)
            .ToArray();

        var variants = _dbContext.Set<CNV.Variant>()
            .AsNoTracking()
            .Include(variant => variant.AffectedTranscripts.Where(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Where(variant => variantIds.Contains(variant.Id))
            .ToArray();

        return variants;
    }

    private SV.Variant[] LoadStructuralVariants(int sampleId, int geneId)
    {
        // Variants should be filtered by the gene the're affecting.
        var variantIds = _dbContext.Set<SV.VariantOccurrence>()
            .AsNoTracking()
            .Where(occurrence => occurrence.AnalysedSample.SampleId == sampleId)
            .Where(occurrence => occurrence.Variant.AffectedTranscripts.Any(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().VariantId)
            .ToArray();

        var variants = _dbContext.Set<SV.Variant>()
            .AsNoTracking()
            .Include(variant => variant.AffectedTranscripts.Where(affectedTranscript => affectedTranscript.Feature.GeneId == geneId))
            .Where(variant => variantIds.Contains(variant.Id))
            .ToArray();

        return variants;
    }


    private GeneExpressionIndex CreateExpressionIndex(int sampleId, int geneId)
    {
        var expression = LoadExpression(sampleId, geneId);

        if (expression == null)
        {
            return null;
        }

        var index = CreateExpressionIndex(expression);

        return index;
    }

    private GeneExpressionIndex CreateExpressionIndex(GeneExpression expression)
    {
        var index = new GeneExpressionIndex();

        _geneExpressionIndexMapper.Map(expression, index);

        return index;
    }

    private GeneExpression LoadExpression(int sampleId, int geneId)
    {
        var expression = _dbContext.Set<GeneExpression>().AsNoTracking().FirstOrDefault(expression =>
            expression.AnalysedSample.SampleId == sampleId &&
            expression.GeneId == geneId
        );

        return expression;
    }
}
