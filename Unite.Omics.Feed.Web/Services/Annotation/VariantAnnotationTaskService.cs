using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Omics.Feed.Web.Services.Annotation;


/// <summary>
/// Variants annotation tasks service.
/// </summary>
/// <typeparam name="TVariant">Variant type.</typeparam>
public abstract class VariantAnnotationTaskService<TVariant> : AnnotationTaskService<TVariant, int>
    where TVariant : Variant
{
    protected override int BucketSize => 1000;


    public VariantAnnotationTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    /// <summary>
    /// Creates only variant annotation tasks for all existing vatiants.
    /// </summary>
    public override void CreateTasks()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        IterateEntities<TVariant, int>(variant => true, variant => variant.Id, variants =>
        {
            CreateVariantAnnotationTasks(variants);
        });

        transaction.Commit();
    }

    /// <summary>
    /// Creates only variant annotation tasks for all variants with given identifiers.
    /// </summary>
    /// <param name="keys">Variants indentifiers.</param>
    public override void CreateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var variants = dbContext.Set<TVariant>()
                .Where(variant => chunk.Contains(variant.Id))
                .Select(variant => variant.Id)
                .ToArray();

            CreateVariantAnnotationTasks(variants);
        });

        transaction.Commit();
    }

    /// <summary>
    /// Populates all types of annotation tasks for variants with given identifiers.
    /// </summary>
    /// <param name="keys">Variants indentifiers.</param>
    public override void PopulateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var variants = dbContext.Set<TVariant>()
                .Where(variant => chunk.Contains(variant.Id))
                .Select(variant => variant.Id)
                .ToArray();

            CreateVariantAnnotationTasks(variants);
        });

        transaction.Commit();
    }


    /// <summary>
    /// Creates annotation tasks for given variant identifiers depedning on variant type.
    /// </summary>
    /// <param name="keys">Variants identifiers.</param>
    private void CreateVariantAnnotationTasks(IEnumerable<int> keys)
    {
        if (typeof(TVariant) == typeof(Unite.Data.Entities.Omics.Analysis.Dna.Sm.Variant))
        {
            CreateTasks(AnnotationTaskType.DNA_SM, keys);
        }
        else if (typeof(TVariant) == typeof(Unite.Data.Entities.Omics.Analysis.Dna.Cnv.Variant))
        {
            CreateTasks(AnnotationTaskType.DNA_CNV, keys);
        }
        else if (typeof(TVariant) == typeof(Unite.Data.Entities.Omics.Analysis.Dna.Sv.Variant))
        {
            CreateTasks(AnnotationTaskType.DNA_SV, keys);
        }
    }
}
