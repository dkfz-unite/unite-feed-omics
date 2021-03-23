using System;
using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Repositories;

namespace Unite.Mutations.Feed.Data.Services
{
    public class VepAnnotationTaskService
    {
        private readonly Repository<Mutation> _mutationRepository;
        private readonly Repository<Task> _taskRepository;

        public VepAnnotationTaskService(UniteDbContext dbContext)
        {
            _mutationRepository = new MutationRepository(dbContext);
            _taskRepository = new TaskRepository(dbContext);
        }

        public void PopulateTasks()
        {
            var position = 0;
            var size = 1000;

            var mutationIds = new List<int>();

            do
            {
                mutationIds = _mutationRepository.Entities
                    .Skip(position)
                    .Take(size)
                    .Select(mutation => mutation.Id)
                    .ToList();

                PopulateMutationAnnotationTasks(mutationIds);

                position += mutationIds.Count();

            }
            while (mutationIds.Count == size);
        }

        public void PopulateTasks(IEnumerable<int> mutationIds)
        {
            PopulateMutationAnnotationTasks(mutationIds);
        }


        private void PopulateMutationAnnotationTasks(IEnumerable<int> mutationIds)
        {
            var tasks = mutationIds.Select(mutationId =>
                CreateTask(mutationId)
            );

            _taskRepository.AddRange(tasks);
        }

        private Task CreateTask(object target)
        {
            var task = new Task
            {
                TypeId = TaskType.AnnotationVEP,
                TargetTypeId = TaskTargetType.Mutation,
                Target = target.ToString(),
                Data = null,
                Date = DateTime.UtcNow
            };

            return task;
        }
    }
}
