using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Data.Services.Tasks;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class GeneIndexingTaskService : IndexingTaskService<Gene, int>
{
    protected override int BucketSize => 1000;


    public GeneIndexingTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }


    public override void CreateTasks()
    {
        var transaction = _dbContext.Database.BeginTransaction();

        IterateEntities<Gene, int>(gene => true, gene => gene.Id, genes =>
        {
            CreateTasks(IndexingTaskType.Gene, genes);
        });

        transaction.Commit();
    }

    public override void CreateTasks(IEnumerable<int> keys)
    {
        var transaction = _dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var genes = _dbContext.Set<Gene>().Where(gene => chunk.Contains(gene.Id)).Select(gene => gene.Id).ToArray();

            CreateTasks(IndexingTaskType.Gene, genes);
        });

        transaction.Commit();
    }

    public override void PopulateTasks(IEnumerable<int> keys)
    {
        var transaction = _dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var genes = _dbContext.Set<Gene>().Where(gene => chunk.Contains(gene.Id)).Select(gene => gene.Id).ToArray();

            CreateDonorIndexingTasks(genes);
            CreateImageIndexingTasks(genes);
            CreateSpecimenIndexingTasks(genes);
            CreateGeneIndexingTasks(genes);
            CreateVariantIndexingTasks(genes);
        });

        transaction.Commit();
    }


    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        var specimenIds = LoadRelatedSpecimens(keys);

        var donorIds = _dbContext.Set<Specimen>()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .Select(specimen => specimen.DonorId)
            .Distinct()
            .ToArray();

        return donorIds;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        var specimeIds = LoadRelatedSpecimens(keys);

        var donorIds = _dbContext.Set<Specimen>()
            .Include(specimen => specimen.Tissue)
            .Where(specimen => specimen.Tissue != null)
            .Where(specimen => specimeIds.Contains(specimen.Id))
            .Select(specimen => specimen.DonorId)
            .Distinct()
            .ToArray();

        var imageIds = _dbContext.Set<Image>()
            .Where(image => donorIds.Contains(image.DonorId))
            .Select(image => image.Id)
            .Distinct()
            .ToArray();

        return imageIds;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        var affectingSsmIds = LoadRelatedMutations(keys);
        var affectingCnvIds = LoadRelatedCopyNumberVariants(keys);
        var affectingSvIds = LoadRelatedStructuralVariants(keys);

        var ssmAffectedSpecimenIds = LoadVariantsAffectedSpecimens<Unite.Data.Entities.Genome.Variants.SSM.Variant, Unite.Data.Entities.Genome.Variants.SSM.VariantOccurrence>(affectingSsmIds);
        var cnvAffectedSpecimenIds = LoadVariantsAffectedSpecimens<Unite.Data.Entities.Genome.Variants.CNV.Variant, Unite.Data.Entities.Genome.Variants.CNV.VariantOccurrence>(affectingCnvIds);
        var svAffectedSpecimenIds = LoadVariantsAffectedSpecimens<Unite.Data.Entities.Genome.Variants.SV.Variant, Unite.Data.Entities.Genome.Variants.SV.VariantOccurrence>(affectingSvIds);
        var expressionAffecredSpecimenIds = LoadExpressionsAffectedSpecimens(keys);

        var specimenIds = ssmAffectedSpecimenIds.Concat(cnvAffectedSpecimenIds).Concat(svAffectedSpecimenIds).Concat(expressionAffecredSpecimenIds).Distinct().ToArray();

        return specimenIds;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return keys;
    }

    protected override IEnumerable<long> LoadRelatedMutations(IEnumerable<int> keys)
    {
        var affectingVariantIds = LoadGenesAffectingVariants<Unite.Data.Entities.Genome.Variants.SSM.Variant, Unite.Data.Entities.Genome.Variants.SSM.AffectedTranscript>(keys);

        return affectingVariantIds;
    }

    protected override IEnumerable<long> LoadRelatedCopyNumberVariants(IEnumerable<int> keys)
    {
        var affectingVariantIds = LoadGenesAffectingVariants<Unite.Data.Entities.Genome.Variants.CNV.Variant, Unite.Data.Entities.Genome.Variants.CNV.AffectedTranscript>(keys);

        return affectingVariantIds;
    }

    protected override IEnumerable<long> LoadRelatedStructuralVariants(IEnumerable<int> keys)
    {
        var affectingVariantIds = LoadGenesAffectingVariants<Unite.Data.Entities.Genome.Variants.SV.Variant, Unite.Data.Entities.Genome.Variants.SV.AffectedTranscript>(keys);

        return affectingVariantIds;
    }


    private IEnumerable<long> LoadGenesAffectingVariants<TVariant, TAffectedFeature>(IEnumerable<int> keys)
        where TAffectedFeature : Unite.Data.Entities.Genome.Variants.VariantAffectedFeature<TVariant, Transcript>
        where TVariant : Unite.Data.Entities.Genome.Variants.Variant
    {
        var affectingVariantIds = _dbContext.Set<TAffectedFeature>()
            .Where(affectedTranscript => keys.Contains(affectedTranscript.Feature.GeneId.Value))
            .Select(affectedTranscript => affectedTranscript.VariantId)
            .Distinct()
            .ToArray();

        return affectingVariantIds;
    }

    private IEnumerable<int> LoadVariantsAffectedSpecimens<TVariant, TVariantOccurrence>(IEnumerable<long> keys)
        where TVariantOccurrence : Unite.Data.Entities.Genome.Variants.VariantOccurrence<TVariant>
        where TVariant : Unite.Data.Entities.Genome.Variants.Variant
    {
        var affectedSpecimenIds = _dbContext.Set<TVariantOccurrence>()
            .Include(variantOccurrence => variantOccurrence.AnalysedSample)
                .ThenInclude(analysedSample => analysedSample.Sample)
            .Where(variantOccurrence => keys.Contains(variantOccurrence.VariantId))
            .Select(variantOccurrence => variantOccurrence.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        return affectedSpecimenIds;
    }

    private IEnumerable<int> LoadExpressionsAffectedSpecimens(IEnumerable<int> keys)
    {
        var affectedSpecimenIds = _dbContext.Set<GeneExpression>()
            .Include(expression => expression.AnalysedSample)
                .ThenInclude(analysedSample => analysedSample.Sample)
            .Where(expression => keys.Contains(expression.GeneId))
            .Select(expression => expression.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        return affectedSpecimenIds;
    }
}
