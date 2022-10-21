using Unite.Data.Services;
using Unite.Data.Services.Tasks;

namespace Unite.Genome.Feed.Web.Services;

/// <summary>
/// Defines abstractions for annotation tasks creation functionality.
/// </summary>
/// <typeparam name="TEntity">Entity type, which has to be annotated.</typeparam>
/// <typeparam name="TKey">Entity key type.</typeparam>
public abstract class AnnotationTaskService<TEntity, TKey> : TaskService
    where TEntity : class
{
    public AnnotationTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }


    /// <summary>
    /// Creates only target type annotation tasks for all existing entities of target type.
    /// </summary>
    public abstract void CreateTasks();

    /// <summary>
    /// Creates only target type annotation tasks for all entities of target type with given identifiers.
    /// </summary>
    /// <param name="keys">Identifiers of entities.</param>
    public abstract void CreateTasks(IEnumerable<TKey> keys);

    /// <summary>
    /// Populates all types of annotation tasks for entities of target type with given identifiers.
    /// </summary>
    /// <param name="keys">Identifiers of entities.</param>
    public abstract void PopulateTasks(IEnumerable<TKey> keys);

}
