using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Web.Services
{
    public class MutationAnnotationTaskService
    {
        private const int BUCKET_SIZE = 1000;

        private readonly DomainDbContext _dbContext;


        public MutationAnnotationTaskService(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// Creates only mutation annotation tasks for all existing mutations
        /// </summary>
        public void CreateTasks()
        {
            IterateMutations(mutation => true, mutations =>
            {
                CreateMutationAnnotationTasks(mutations);
            });
        }

        /// <summary>
        /// Creates only mutation annotation tasks for all mutations with given identifiers
        /// </summary>
        /// <param name="mutationIds">Identifiers of mutations</param>
        public void CreateTasks(IEnumerable<long> mutationIds)
        {
            IterateMutations(mutation => mutationIds.Contains(mutation.Id), mutations =>
            {
                CreateMutationAnnotationTasks(mutations);
            });
        }

        /// <summary>
        /// Populates all types of annotation tasks for mutations with given identifiers
        /// </summary>
        /// <param name="mutationIds">Identifiers of mutations</param>
        public void PopulateTasks(IEnumerable<long> mutationIds)
        {
            IterateMutations(mutation => mutationIds.Contains(mutation.Id), mutations =>
            {
                CreateMutationAnnotationTasks(mutations);
            });
        }


        private void CreateMutationAnnotationTasks(IEnumerable<long> mutationIds)
        {
            var tasks = mutationIds.Select(mutationId =>
            {
                var task = new Task
                {
                    TypeId = TaskType.Annotation,
                    TargetTypeId = TaskTargetType.Mutation,
                    Target = mutationId.ToString(),
                    Data = null,
                    Date = DateTime.UtcNow
                };

                return task;

            }).ToArray();

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.SaveChanges();
        }

        private void IterateMutations(Expression<Func<Mutation, bool>> condition, Action<IEnumerable<long>> handler)
        {
            var position = 0;

            var mutations = Enumerable.Empty<long>();

            do
            {
                mutations = _dbContext.Mutations
                    .Where(condition)
                    .Skip(position)
                    .Take(BUCKET_SIZE)
                    .Select(mutation => mutation.Id)
                    .ToArray();

                handler.Invoke(mutations);

                position += mutations.Count();

            }
            while (mutations.Count() == BUCKET_SIZE);
        }
    }
}
