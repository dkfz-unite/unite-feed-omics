using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Genes;
using Unite.Mapping;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;


namespace Unite.Genome.Indices.Services;

public class GeneIndexCreationService
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly GeneIndexCreationContextLoader _contextLoader;


    public GeneIndexCreationService(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _contextLoader = new GeneIndexCreationContextLoader(dbContextFactory);
    }


    public GeneIndex CreateIndex(object key)
    {
        var geneId = (int)key;

        return CreateGeneIndex(geneId);
    }


    private GeneIndex CreateGeneIndex(int geneId)
    {
        var context = _contextLoader.LoadContext(geneId);

        if (context.Gene == null || context.DonorsCache.IsEmpty())
            return null;

        return CreateGeneIndex(context);
    }

    private GeneIndex CreateGeneIndex(GeneIndexCreationContext context)
    {
        var index = GeneIndexMapper.CreateFrom<GeneIndex>(context.Gene);

        index.Specimens = CreateSpecimenIndices(context);

        // If gene doesn't affect any specimens it should be removed.
        if (index.Specimens.IsEmpty())
            return null;

        return index;
    }


    private SpecimenIndex[] CreateSpecimenIndices(GeneIndexCreationContext context)
    {
        var specimens = LoadSpecimens(context);

        return specimens.Select(specimen => CreateSpecimenIndex(specimen, context)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, GeneIndexCreationContext context)
    {
        var diagnosisDate = context.DonorsCache[specimen.Id].ClinicalData?.DiagnosisDate;

        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, diagnosisDate);

        index.Donor = CreateDonorIndex(specimen.Id, context);
        index.Images = CreateImageIndices(specimen.Id, context, diagnosisDate);
        index.Samples = CreateSampleIndices(specimen.Id, context, diagnosisDate);
        index.Variants = CreateVariantIndices(specimen.Id, context);
        
        return index;
    }

    private static Specimen[] LoadSpecimens(GeneIndexCreationContext context)
    {
        return context.SpecimensCache.Values.ToArray();
    }


    private static DonorIndex CreateDonorIndex(int specimenId, GeneIndexCreationContext context)
    {
        var donor = LoadDonor(specimenId, context);

        if (donor == null)
            return null;

        return CreateDonorIndex(donor);
    }

    private static DonorIndex CreateDonorIndex(Donor donor)
    {
        return DonorIndexMapper.CreateFrom<DonorIndex>(donor);
    }

    private static Donor LoadDonor(int specimenId, GeneIndexCreationContext context)
    {
        return context.DonorsCache.TryGetValue(specimenId, out var value) ? value : null;
    }


    private static ImageIndex[] CreateImageIndices(int specimenId, GeneIndexCreationContext context, DateOnly? diagnosisDate)
    {
        var images = LoadImages(specimenId, context);
        
        return images.Select(entity => CreateImageIndex(entity, diagnosisDate)).ToArrayOrNull();
    }

    private static ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        return ImageIndexMapper.CreateFrom<ImageIndex>(image, diagnosisDate);
    }

    private static Image[] LoadImages(int specimenId, GeneIndexCreationContext context)
    {
        return context.ImagesCache.TryGetValue(specimenId, out var value) ? value : [];
    }


    private static SampleIndex[] CreateSampleIndices(int specimenId, GeneIndexCreationContext context, DateOnly? diagnosisDate)
    {
        var samples = LoadSamples(specimenId, context);

        return samples.Select(sample => CreateSampleIndex(sample, diagnosisDate)).ToArrayOrNull();
    }

    private static SampleIndex CreateSampleIndex(Sample sample, DateOnly? diagnosisDate)
    {
        return SampleIndexMapper.CreateFrom<SampleIndex>(sample, diagnosisDate);
    }

    private static Sample[] LoadSamples(int specimenId, GeneIndexCreationContext context)
    {
        return context.Samples.TryGetValue(specimenId, out var value) ? value : [];
    }


    private VariantIndex[] CreateVariantIndices(int specimenId, GeneIndexCreationContext context)
    {
        var indices = new List<VariantIndex>();

        LoadSsms(specimenId, context).ForEach(variant => indices.Add(CreateVariantIndex(variant)));
        LoadCnvs(specimenId, context).ForEach(variant => indices.Add(CreateVariantIndex(variant)));
        LoadSvs(specimenId, context).ForEach(variant => indices.Add(CreateVariantIndex(variant)));

        return indices.ToArrayOrNull();
    }

    private static VariantIndex CreateVariantIndex<TVariant>(TVariant variant) where TVariant : Variant
    {
        return VariantIndexMapper.CreateFrom<VariantIndex>(variant);
    }

    private SSM.Variant[] LoadSsms(int specimenId, GeneIndexCreationContext context)
    {
        var variants = LoadVariants<SSM.VariantEntry, SSM.Variant>(specimenId, context.SsmAffectedTranscriptsCache.Keys);

        variants.ForEach(variant => 
        {
            variant.AffectedTranscripts = context.SsmAffectedTranscriptsCache.TryGetValue(variant.Id, out var value) ? value : null;
        });  

        return variants;
    }

    private CNV.Variant[] LoadCnvs(int specimenId, GeneIndexCreationContext context)
    {
        var variants = LoadVariants<CNV.VariantEntry, CNV.Variant>(specimenId, context.CnvAffectedTranscriptsCache.Keys);

        variants.ForEach(variant => 
        {
            variant.AffectedTranscripts = context.CnvAffectedTranscriptsCache.TryGetValue(variant.Id, out var value) ? value : null;
        });

        return variants;
    }

    private SV.Variant[] LoadSvs(int specimenId, GeneIndexCreationContext context)
    {
        var variants = LoadVariants<SV.VariantEntry, SV.Variant>(specimenId, context.SvAffectedTranscriptsCache.Keys);

        variants.ForEach(variant => 
        {
            variant.AffectedTranscripts = context.SvAffectedTranscriptsCache.TryGetValue(variant.Id, out var value) ? value : null;
        });

        return variants;
    }

    private TVariant[] LoadVariants<TVariantEntry, TVariant>(int specimenId, IEnumerable<int> affectedTranscriptsCache)
        where TVariantEntry : VariantEntry<TVariant>
        where TVariant : Variant
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<TVariantEntry>()
            .AsNoTracking()
            .Include(entry => entry.Entity)
            .Where(entry => entry.Sample.SpecimenId == specimenId)
            .Where(entry => affectedTranscriptsCache.Contains(entry.EntityId))
            .GroupBy(entry => entry.EntityId)
            .Select(group => group.First().Entity)
            .ToArray();
    }
}
