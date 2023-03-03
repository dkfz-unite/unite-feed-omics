using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens.Tissues.Enums;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Data.Services.Tasks;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public abstract class VariantIndexingTaskService<TVariant, TVariantOccurrence, TAffectedTranscript> : IndexingTaskService<Variant, long>
    where TVariant : Variant
    where TVariantOccurrence : VariantOccurrence<TVariant>
    where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
{
    protected override int BucketSize => 1000;


    public VariantIndexingTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }


    public override void CreateTasks()
    {
        var transaction = _dbContext.Database.BeginTransaction();

        IterateEntities<TVariant, long>(variant => true, variant => variant.Id, variants =>
        {
            CreateVariantIndexingTasks(variants);
        });

        transaction.Commit();
    }

    public override void CreateTasks(IEnumerable<long> keys)
    {
        var transaction = _dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var variants = _dbContext.Set<TVariant>().Where(variant => chunk.Contains(variant.Id)).Select(variant => variant.Id).ToArray();

            CreateVariantIndexingTasks(variants);
        });

        transaction.Commit();
    }

    public override void PopulateTasks(IEnumerable<long> keys)
    {
        var transaction = _dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var variants = _dbContext.Set<TVariant>().Where(variant => chunk.Contains(variant.Id)).Select(variant => variant.Id).ToArray();

            CreateDonorIndexingTasks(variants);
            CreateImageIndexingTasks(keys);
            CreateSpecimenIndexingTasks(variants);
            CreateGeneIndexingTasks(variants);
            CreateVariantIndexingTasks(variants);
        });

        transaction.Commit();
    }


    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<long> keys)
    {
        var donorIds = _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => keys.Contains(occurrence.VariantId))
            .Select(occurrence => occurrence.AnalysedSample.Sample.Specimen.DonorId)
            .Distinct()
            .ToArray();

        return donorIds;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<long> keys)
    {
        var donorIds = _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => keys.Contains(occurrence.VariantId))
            .Where(occurrence => occurrence.AnalysedSample.Sample.Specimen.Tissue.TypeId == TissueType.Tumor)
            .Select(occurrence => occurrence.AnalysedSample.Sample.Specimen.DonorId)
            .Distinct()
            .ToArray();

        var imageIds = _dbContext.Set<Image>()
            .Where(image => donorIds.Contains(image.DonorId))
            .Select(image => image.Id)
            .Distinct()
            .ToArray();

        return imageIds;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<long> keys)
    {
        var specimenIds = _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => keys.Contains(occurrence.VariantId))
            .Select(occurrence => occurrence.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        return specimenIds;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<long> keys)
    {
        var geneIds = _dbContext.Set<TAffectedTranscript>()
            .Where(affectedTranscript => keys.Contains(affectedTranscript.VariantId))
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId != null)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        return geneIds;
    }

    protected override IEnumerable<long> LoadRelatedMutations(IEnumerable<long> keys)
    {
        if (typeof(TVariant) == typeof(Unite.Data.Entities.Genome.Variants.SSM.Variant))
        {
            return keys;
        }
        else
        {
            return Enumerable.Empty<long>();
        }
    }

    protected override IEnumerable<long> LoadRelatedCopyNumberVariants(IEnumerable<long> keys)
    {
        if (typeof(TVariant) == typeof(Unite.Data.Entities.Genome.Variants.CNV.Variant))
        {
            return keys;
        }
        else
        {
            return Enumerable.Empty<long>();
        }
    }

    protected override IEnumerable<long> LoadRelatedStructuralVariants(IEnumerable<long> keys)
    {
        if (typeof(TVariant) == typeof(Unite.Data.Entities.Genome.Variants.SV.Variant))
        {
            return keys;
        }
        else
        {
            return Enumerable.Empty<long>();
        }
    }
}
