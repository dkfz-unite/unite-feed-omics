using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Web.Services;


/// <summary>
/// Variants annotation tasks service.
/// </summary>
/// <typeparam name="TVariant">Variant type.</typeparam>
public abstract class VariantAnnotationTaskService<TVariant> : AnnotationTaskService<TVariant, long>
    where TVariant : Variant
{
    protected override int BucketSize => 1000;


    public VariantAnnotationTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }


    /// <summary>
    /// Creates only variant annotation tasks for all existing vatiants.
    /// </summary>
    public override void CreateTasks()
    {
        IterateEntities<TVariant, long>(variant => true, variant => variant.Id, variants =>
        {
            CreateVariantAnnotationTasks(variants);
        });
    }

    /// <summary>
    /// Creates only variant annotation tasks for all variants with given identifiers.
    /// </summary>
    /// <param name="variantIds">Variants indentifiers.</param>
    public override void CreateTasks(IEnumerable<long> variantIds)
    {
        IterateEntities<TVariant, long>(variant => variantIds.Contains(variant.Id), variant => variant.Id, variants =>
        {
            CreateVariantAnnotationTasks(variants);
        });
    }

    /// <summary>
    /// Populates all types of annotation tasks for variants with given identifiers.
    /// </summary>
    /// <param name="variantIds">Variants indentifiers.</param>
    public override void PopulateTasks(IEnumerable<long> variantIds)
    {
        IterateEntities<TVariant, long>(variant => variantIds.Contains(variant.Id), variant => variant.Id, variants =>
        {
            CreateVariantAnnotationTasks(variants);
        });
    }


    /// <summary>
    /// Creates annotation tasks for given variant identifiers depedning on variant type.
    /// </summary>
    /// <param name="variantIds">Variants identifiers.</param>
    private void CreateVariantAnnotationTasks(IEnumerable<long> variantIds)
    {
        if (typeof(TVariant) == typeof(Unite.Data.Entities.Genome.Variants.SSM.Variant))
        {
            CreateTasks(AnnotationTaskType.SSM, variantIds);
        }
        else if (typeof(TVariant) == typeof(Unite.Data.Entities.Genome.Variants.CNV.Variant))
        {
            CreateTasks(AnnotationTaskType.CNV, variantIds);
        }
        else if (typeof(TVariant) == typeof(Unite.Data.Entities.Genome.Variants.SV.Variant))
        {
            CreateTasks(AnnotationTaskType.SV, variantIds);
        }
    }
}
