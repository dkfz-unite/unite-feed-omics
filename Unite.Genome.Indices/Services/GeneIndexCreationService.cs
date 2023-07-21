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

    private class Context
    {
        /// <summary>
        /// Gene
        /// </summary>
        public Gene Gene;

        /// <summary>
        /// Map of variant id to affected transcripts
        /// </summary>
        public IDictionary<long, SSM.AffectedTranscript[]> SsmAffectedTranscriptsCache;

        /// <summary>
        /// Map of variant id to affected transcripts
        /// </summary>
        public IDictionary<long, CNV.AffectedTranscript[]> CnvAffectedTranscriptsCache;

        /// <summary>
        /// Map of variant id to affected transcripts
        /// </summary>
        public IDictionary<long, SV.AffectedTranscript[]> SvAffectedTranscriptsCache;

        /// <summary>
        /// Map of sample id to gene expression
        /// </summary>
        public IDictionary<int, GeneExpression> ExpressionsCache;

        /// <summary>
        /// Map of specimen id to donor
        /// </summary>
        public IDictionary<int, Donor> DonorsCache;

        /// <summary>
        /// Map of specimen id to images
        /// </summary>
        public IDictionary<int, Image[]> ImagesCache;

        /// <summary>
        /// Map of specimen id to specimen
        /// </summary>
        public IDictionary<int, Specimen> SpecimensCache;
    }


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
        // Expression<Func<SSM.AffectedTranscript, bool>> ssmsPredicate = (affectedTranscript) => 
        //     affectedTranscript.Feature.GeneId == geneId;

        // Expression<Func<CNV.AffectedTranscript, bool>> cnvsPredicate = (affectedTranscript) =>
        //     affectedTranscript.Feature.GeneId == geneId;

        // Expression<Func<SV.AffectedTranscript, bool>> svsPredicate = (affectedTranscript) =>
        //     affectedTranscript.Feature.GeneId == geneId;

        var context = new Context
        {
            Gene = _dbContext.Set<Gene>().AsNoTracking().FirstOrDefault(gene => gene.Id == geneId),
            SsmAffectedTranscriptsCache = LoadAffectedTranscripts<SSM.Variant, SSM.AffectedTranscript>(geneId),
            CnvAffectedTranscriptsCache = LoadAffectedTranscripts<CNV.Variant, CNV.AffectedTranscript>(geneId),
            SvAffectedTranscriptsCache = LoadAffectedTranscripts<SV.Variant, SV.AffectedTranscript>(geneId),
            ExpressionsCache = LoadExpressions(geneId)
        };

        return context;
    }

    // private IDictionary<long, TAffectedTranscript[]> LoadAffectedTranscripts<TVariant, TAffectedTranscript>(Expression<Func<TAffectedTranscript, bool>> predicate)
    //     where TVariant : Variant
    //     where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
    // {
    //     var affectedTranscripts = _dbContext.Set<TAffectedTranscript>()
    //         .AsNoTracking()
    //         .Include(affectedTranscript => affectedTranscript.Feature)
    //         .Where(predicate)
    //         .ToArray();

    //     var affectedTranscriptsCache = affectedTranscripts
    //         .GroupBy(affectedTranscript => affectedTranscript.VariantId)
    //         .ToDictionary(group => group.Key, group => group.ToArray());

    //     return affectedTranscriptsCache;
    // }

    private IDictionary<long, TAffectedTranscript[]> LoadAffectedTranscripts<TVariant, TAffectedTranscript>(int geneId)
        where TVariant : Variant
        where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
    {
        var affectedTranscripts = _dbContext.Set<TAffectedTranscript>()
            .AsNoTracking()
            .Include(affectedTranscript => affectedTranscript.Feature.Protein)
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
            .Include(expression => expression.AnalysedSample)
            .Where(expresson => expresson.GeneId == geneId)
            .ToArray();

        var expressionsCache = expressons
            .GroupBy(expression => expression.AnalysedSample.SampleId)
            .ToDictionary(group => group.Key, group => group.FirstOrDefault());

        return expressionsCache;
    }


    private SampleIndex[] CreateSampleIndices(Context context)
    {
        var samples = LoadSamples(context);
        context.DonorsCache = LoadDonorsCache(samples.Select(sample => sample.Sample.SpecimenId));
        context.ImagesCache = LoadImagesCache(samples.Select(sample => sample.Sample.SpecimenId));
        context.SpecimensCache = LoadSpecimensCache(samples.Select(sample => sample.Sample.SpecimenId));

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

        index.Donor = CreateDonorIndex(context, sample.SpecimenId, out var donor);

        index.Specimen = CreateSpecimenIndex(context, sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

        index.Images = CreateImageIndices(context, sample.SpecimenId, donor.ClinicalData?.DiagnosisDate);

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
        var expressionSampleIds = context.ExpressionsCache.Keys.ToArray();

        var analysedSampleIds = Enumerable.Empty<int>()
            .Union(ssmAffectedSampleIds)
            .Union(cnvAffectedSampleIds)
            .Union(svAffectedSampleIds)
            .Union(expressionSampleIds)
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
                var expression = context.ExpressionsCache.TryGetValue(group.Key, out var value) ? value : null;
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

    private IDictionary<int, Donor> LoadDonorsCache(IEnumerable<int> specimenIds)
    {
        var specimensToDonorsMap = _dbContext.Set<Specimen>().AsNoTracking()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .Select(specimen => new { specimen.Id, specimen.DonorId }).ToArray()
            .ToDictionary(entry => entry.Id, entry => entry.DonorId);

        var donorsMap = _dbContext.Set<Donor>().AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeStudies()
            .IncludeProjects()
            .Where(donor => specimensToDonorsMap.Values.Distinct().Contains(donor.Id))
            .ToArray().ToDictionary(donor => donor.Id, donor => donor);

        return specimensToDonorsMap.ToDictionary(map => map.Key, map => donorsMap.TryGetValue(map.Value, out var value) ? value : null);
    }

    private IDictionary<int, Specimen> LoadSpecimensCache(IEnumerable<int> specimenids)
    {
        var specimensMap = _dbContext.Set<Specimen>().AsNoTracking()
            .IncludeTissue()
            .IncludeCellLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .Where(specimen => specimenids.Contains(specimen.Id))
            .ToArray().ToDictionary(specimen => specimen.Id, specimen => specimen);

        return specimensMap;
    }

    private IDictionary<int, Image[]> LoadImagesCache(IEnumerable<int> specimenIds)
    {
        var specimensToDonorsMap = _dbContext.Set<Specimen>().AsNoTracking()
            .Where(specimen => specimen.Tissue.TypeId == TissueType.Tumor)
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .Select(specimen => new { specimen.Id, specimen.DonorId }).ToArray()
            .ToDictionary(entry => entry.Id, entry => entry.DonorId);

        var donorsToImagesMap = _dbContext.Set<Image>().AsNoTracking()
            .Include(image => image.MriImage)
            .Where(image => specimensToDonorsMap.Values.Distinct().Contains(image.DonorId))
            .ToArray().GroupBy(image => image.DonorId)
            .ToDictionary(group => group.Key, group => group.ToArray());

        return specimensToDonorsMap.ToDictionary(map => map.Key, map => donorsToImagesMap.TryGetValue(map.Value, out var value) ? value : null);
    }


    private DonorIndex CreateDonorIndex(Context context, int specimenId, out Donor donor)
    {
        donor = LoadDonor(context, specimenId);

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

    private Donor LoadDonor(Context context, int specimenId)
    {
        return context.DonorsCache.TryGetValue(specimenId, out var value) ? value : null;
    }


    private SpecimenIndex CreateSpecimenIndex(Context context, int specimenId, DateOnly? diagnosisDate)
    {
        var specimen = LoadSpecimen(context, specimenId);

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

    private Specimen LoadSpecimen(Context context, int specimenId)
    {
        return context.SpecimensCache.TryGetValue(specimenId, out var value) ? value : null;
    }


    private ImageIndex[] CreateImageIndices(Context context, int specimenId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(context, specimenId);

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

    private Image[] LoadImages(Context context, int specimenId)
    {
        return context.ImagesCache.TryGetValue(specimenId, out var value) ? value : null;
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
            variant.AffectedTranscripts = context.SsmAffectedTranscriptsCache.TryGetValue(variant.Id, out var value) ? value : null;
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
            variant.AffectedTranscripts = context.CnvAffectedTranscriptsCache.TryGetValue(variant.Id, out var value) ? value : null;
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
            variant.AffectedTranscripts = context.SvAffectedTranscriptsCache.TryGetValue(variant.Id, out var value) ? value : null;
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
        return context.ExpressionsCache.TryGetValue(sampleId, out var value) ? value : null;
    }
}
