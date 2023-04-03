using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Entities.Genome.Variants;
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

    private record Context
    (
        Gene Gene,
        IDictionary<long, SSM.AffectedTranscript[]> SsmAffectedTranscriptsCache,
        IDictionary<long, CNV.AffectedTranscript[]> CnvAffectedTranscriptsCache,
        IDictionary<long, SV.AffectedTranscript[]> SvAffectedTranscriptsCache,
        IDictionary<int, GeneExpression> ExpressionsCache
    );


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
        var context = LoadContext(geneId);

        if (context.Gene == null)
        {
            return null;
        }

        var index = CreateGeneIndex(context);

        return index;
    }

    private GeneIndex CreateGeneIndex(Context context)
    {
        var index = new GeneIndex();

        _geneIndexMapper.Map(context.Gene, index);

        index.Samples = CreateSampleIndices(context);

        return index;
    }

    private Context LoadContext(int geneId)
    {
        var gene = _dbContext.Set<Gene>().AsNoTracking().FirstOrDefault(gene => gene.Id == geneId);
        var ssmAffectedTranscriptsCache = LoadAffectedTranscripts<SSM.Variant, SSM.AffectedTranscript>(geneId);
        var cnvAffectedTranscriptsCache = LoadAffectedTranscripts<CNV.Variant, CNV.AffectedTranscript>(geneId);
        var svAffectedTranscriptsCache = LoadAffectedTranscripts<SV.Variant, SV.AffectedTranscript>(geneId);
        var expressionsCache = LoadExpressions(geneId);

        return new (gene, ssmAffectedTranscriptsCache, cnvAffectedTranscriptsCache, svAffectedTranscriptsCache, expressionsCache);
    }

    private IDictionary<long, TAffectedTranscript[]> LoadAffectedTranscripts<TVariant, TAffectedTranscript>(int geneId)
        where TVariant : Variant
        where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
    {
        var affectedTranscripts = _dbContext.Set<TAffectedTranscript>()
            .AsNoTracking()
            .Include(affectedTranscript => affectedTranscript.Feature)
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId == geneId)
            .ToArray();

        var affectedTranscriptsCache = affectedTranscripts
            .GroupBy(affectedTranscript => affectedTranscript.VariantId)
            .ToDictionary(group => group.Key, group => group.ToArray());

        return affectedTranscriptsCache;
    }

    private IDictionary<int, GeneExpression> LoadExpressions(int geneId)
    {
        var expressons = _dbContext.Set<GeneExpression>().AsNoTracking()
            .Where(expresson => expresson.GeneId == geneId)
            .GroupBy(expression => expression.AnalysedSample.SampleId)
            .ToArray();

        var expressionsCache = expressons.ToDictionary(group => group.Key, group => group.FirstOrDefault());

        return expressionsCache;
    }


    private SampleIndex[] CreateSampleIndices(Context context)
    {
        var samples = LoadSamples(context);

        if (samples == null)
        {
            return null;
        }

        var indices = samples
            .Select(sample => CreateSampleIndex(context, sample.Sample, sample.Analyses, sample.expression))
            .ToArray();

        return indices;
    }

    private SampleIndex CreateSampleIndex(Context context, Sample sample, Analysis[] analyses, GeneExpression expression)
    {
        var index = new SampleIndex();

        index.Donor = CreateDonorIndex(sample.SpecimenId, out var donor);

        index.Specimen = CreateSpecimenIndex(sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        index.Images = CreateImageIndices(sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        index.Variants = CreateVariantIndices(context, sample.Id);

        index.Expression = CreateExpressionIndex(context, sample.Id);

        _sampleIndexMapper.Map(sample, analyses, index, donor.ClinicalData?.DiagnosisDate);

        return index;
    }

    private (Sample Sample, Analysis[] Analyses, GeneExpression expression)[] LoadSamples(Context context)
    {
        var ssmAffectedSampleIds = LoadAnalysedSampleIds<SSM.Variant, SSM.VariantOccurrence, SSM.AffectedTranscript>(context.SsmAffectedTranscriptsCache);
        var cnvAffectedSampleIds = LoadAnalysedSampleIds<CNV.Variant, CNV.VariantOccurrence, CNV.AffectedTranscript>(context.CnvAffectedTranscriptsCache);
        var svAffectedSampleIds = LoadAnalysedSampleIds<SV.Variant, SV.VariantOccurrence, SV.AffectedTranscript>(context.SvAffectedTranscriptsCache);

        var analysedSampleIds = Enumerable.Empty<int>()
            .Union(ssmAffectedSampleIds)
            .Union(cnvAffectedSampleIds)
            .Union(svAffectedSampleIds)
            .ToArray();

        var analysedSamples = _dbContext.Set<AnalysedSample>()
            .AsNoTracking()
            .Include(analysedSample => analysedSample.Sample)
            .Include(analysedSample => analysedSample.Analysis)
            .Where(analysedSample => analysedSampleIds.Contains(analysedSample.Id))
            .ToArray();

        var samples = analysedSamples
            .GroupBy(analysedSample => analysedSample.SampleId)
            .Select(group => {
                var sample = group.First().Sample;
                var analyses = group.Select(sample => sample.Analysis).ToArray();
                var expression = context.ExpressionsCache.ContainsKey(group.Key) ? context.ExpressionsCache[group.Key] : null;
                return (sample, analyses, expression);
            })
            .ToArray();

        return samples;
    }

    private int[] LoadAnalysedSampleIds<TVariant, TVariantOccurrence, TAffectedTranscript>(IDictionary<long, TAffectedTranscript[]> affectedTranscriptsCache)
        where TVariant : Variant
        where TVariantOccurrence : VariantOccurrence<TVariant>
        where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
    {
        if (affectedTranscriptsCache?.Any() == true)
        {
            var analysedSampleIds = _dbContext.Set<TVariantOccurrence>().AsNoTracking()
                .Where(occurrence => affectedTranscriptsCache.Keys.Contains(occurrence.VariantId))
                .Select(occurrence => occurrence.AnalysedSampleId)
                .Distinct()
                .ToArray();
            
            return analysedSampleIds;
        }
        else
        {
            return Array.Empty<int>();
        }
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


    private VariantIndex[] CreateVariantIndices(Context context, int sampleId)
    {
        var mutations = LoadMutations(context, sampleId);
        var copyNumberVariants = LoadCopyNumberVariants(context, sampleId);
        var structuralVariants = LoadStructuralVariants(context, sampleId);

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

    private SSM.Variant[] LoadMutations(Context context, int sampleId)
    {
        var variants = _dbContext.Set<SSM.VariantOccurrence>().AsNoTracking()
            .Include(occurrence => occurrence.Variant)
            .Where(occurrence => occurrence.AnalysedSample.SampleId == sampleId)
            .Where(occurrence => context.SsmAffectedTranscriptsCache.Keys.Contains(occurrence.VariantId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().Variant)
            .ToArray();

        foreach (var variant in variants)
        {
            variant.AffectedTranscripts = context.SsmAffectedTranscriptsCache.ContainsKey(variant.Id) ? context.SsmAffectedTranscriptsCache[variant.Id] : null;
        }

        return variants;
    }

    private CNV.Variant[] LoadCopyNumberVariants(Context context, int sampleId)
    {
        var variants = _dbContext.Set<CNV.VariantOccurrence>().AsNoTracking()
            .Include(occurrence => occurrence.Variant)
            .Where(occurrence => occurrence.AnalysedSample.SampleId == sampleId)
            .Where(occurrence => context.CnvAffectedTranscriptsCache.Keys.Contains(occurrence.VariantId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().Variant)
            .ToArray();

        foreach (var variant in variants)
        {
            variant.AffectedTranscripts = context.CnvAffectedTranscriptsCache.ContainsKey(variant.Id) ? context.CnvAffectedTranscriptsCache[variant.Id] : null;
        }

        return variants;
    }

    private SV.Variant[] LoadStructuralVariants(Context context, int sampleId)
    {
        var variants = _dbContext.Set<SV.VariantOccurrence>().AsNoTracking()
            .Include(occurrence => occurrence.Variant)
            .Where(occurrence => occurrence.AnalysedSample.SampleId == sampleId)
            .Where(occurrence => context.SvAffectedTranscriptsCache.Keys.Contains(occurrence.VariantId))
            .GroupBy(occurrence => occurrence.VariantId)
            .Select(group => group.First().Variant)
            .ToArray();

        foreach (var variant in variants)
        {
            variant.AffectedTranscripts = context.SvAffectedTranscriptsCache.ContainsKey(variant.Id) ? context.SvAffectedTranscriptsCache[variant.Id] : null;
        }

        return variants;
    }


    private GeneExpressionIndex CreateExpressionIndex(Context context, int sampleId)
    {
        var expression = LoadExpression(context, sampleId);

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

    private GeneExpression LoadExpression(Context context, int sampleId)
    {
        var expression = context.ExpressionsCache.ContainsKey(sampleId) ? context.ExpressionsCache[sampleId] : null;

        return expression;
    }
}
