using System;
using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Repositories;

namespace Unite.Mutations.Feed.Data.Services.Mutations
{
    public class MutationIndexingTaskService
    {
        private readonly Repository<Mutation> _mutationRepository;
        private readonly Repository<MutationOccurrence> _mutationOccurrenceRepository;
        private readonly Repository<Task> _taskRepository;

        public MutationIndexingTaskService(UniteDbContext dbContext)
        {
            _mutationRepository = new MutationRepository(dbContext);
            _mutationOccurrenceRepository = new MutationOccurrenceRepository(dbContext);

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

                PopulateMutationIndexingTasks(mutationIds);

                position += mutationIds.Count();

            }
            while (mutationIds.Count == size);
        }

        public void PopulateTasks(IEnumerable<int> mutationIds)
        {
            PopulateMutationIndexingTasks(mutationIds);

            PopulateDonorIndexingTasks(mutationIds);
        }


        private void PopulateMutationIndexingTasks(IEnumerable<int> mutationIds)
        {
            var mutationIndexingTasks = mutationIds.Select(mutationId =>
                CreateTask(TaskTargetType.Mutation, mutationId)
            );

            _taskRepository.AddRange(mutationIndexingTasks);
        }

        private void PopulateDonorIndexingTasks(IEnumerable<int> mutationIds)
        {
            var donorIndexingTasks = mutationIds
                .SelectMany(mutationId => AffectedDonors(mutationId))
                .Distinct()
                .Select(donorId => CreateTask(TaskTargetType.Donor, donorId));

            _taskRepository.AddRange(donorIndexingTasks);
        }

        private Task CreateTask(TaskTargetType targetType, object target)
        {
            var task = new Task
            {
                TypeId = TaskType.Indexing,
                TargetTypeId = targetType,
                Target = target.ToString(),
                Data = null,
                Date = DateTime.UtcNow
            };

            return task;
        }


        private IEnumerable<string> AffectedDonors(int mutationId)
        {
            return _mutationOccurrenceRepository.Entities
                .Where(entity => entity.MutationId == mutationId)
                .Select(entity => entity.AnalysedSample.Sample.DonorId)
                .Distinct()
                .ToArray();
        }
    }
}
