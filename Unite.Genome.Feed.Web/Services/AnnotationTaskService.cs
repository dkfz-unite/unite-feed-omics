using System.Collections.Generic;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Web.Services
{
    public abstract class AnnotationTaskService<T, TKey> : TaskService
        where T : class
    {
        public AnnotationTaskService(DomainDbContext dbContext) : base(dbContext)
        {
        }


        /// <summary>
        /// Creates only target type annotation tasks for all existing entities of target type
        /// </summary>
        public abstract void CreateTasks();

        /// <summary>
        /// Creates only target type annotation tasks for all entities of target type with given identifiers
        /// </summary>
        /// <param name="keys">Identifiers of entities</param>
        public abstract void CreateTasks(IEnumerable<TKey> keys);

        /// <summary>
        /// Populates all types of annotation tasks for entities of target type with given identifiers
        /// </summary>
        /// <param name="keys">Identifiers of entities</param>
        public abstract void PopulateTasks(IEnumerable<TKey> keys);

    }
}
