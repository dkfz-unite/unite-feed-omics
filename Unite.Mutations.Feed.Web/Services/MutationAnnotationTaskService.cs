using System;
using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Web.Services
{
    public class MutationAnnotationTaskService
    {
        private const int BUCKET_SIZE = 1000;

        private readonly UniteDbContext _dbContext;


        public MutationAnnotationTaskService(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void PopulateTasks(IEnumerable<long> mutationIds)
        {
            var position = 0;

            var mutations = Enumerable.Empty<Mutation>();

            do
            {
                mutations = _dbContext.Mutations
                    .Where(mutation => mutationIds.Contains(mutation.Id))
                    .Skip(position)
                    .Take(BUCKET_SIZE)
                    .ToArray();

                PopulateAnnotationTasks(mutations);

                position += mutations.Count();

            }
            while (mutations.Count() == BUCKET_SIZE);
        }


        private void PopulateAnnotationTasks(IEnumerable<Mutation> mutations)
        {
            var tasks = mutations.Select(mutation =>
            {
                var task = new Task
                {
                    TypeId = TaskType.Annotation,
                    TargetTypeId = TaskTargetType.Mutation,
                    Target = mutation.Id.ToString(),
                    Data = null,
                    Date = DateTime.UtcNow
                };

                return task;

            }).ToArray();

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.SaveChanges();
        }
    }
}
