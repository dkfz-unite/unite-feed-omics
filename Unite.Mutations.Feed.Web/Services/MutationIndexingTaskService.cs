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
    public class MutationIndexingTaskService
    {
        private const int BUCKET_SIZE = 1000;

        private readonly UniteDbContext _dbContext;


        public MutationIndexingTaskService(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// Creates only mutation indexing tasks for all existing mutations
        /// </summary>
        public void CreateTasks()
        {
            IterateMutations(mutation => true, mutations =>
            {
                CreateMutationIndexingTasks(mutations);
            });
        }

        /// <summary>
        /// Creates only mutation indexing tasks for all mutations with given identifiers
        /// </summary>
        /// <param name="mutationIds">Identifiers of mutations</param>
        public void CreateTasks(IEnumerable<long> mutationIds)
        {
            IterateMutations(mutation => mutationIds.Contains(mutation.Id), mutations =>
            {
                CreateMutationIndexingTasks(mutations);
            });
        }

        /// <summary>
        /// Populates all types of indexing tasks for mutations with given identifiers
        /// </summary>
        /// <param name="mutationIds">Identifiers of mutations</param>
        public void PopulateTasks(IEnumerable<long> mutationIds)
        {
            IterateMutations(mutation => mutationIds.Contains(mutation.Id), mutations =>
            {
                CreateMutationIndexingTasks(mutations);
                CreateDonorIndexingTasks(mutations);
                //CreateSpecimenIndexingTasks(mutations);
            });
        }


        private void CreateMutationIndexingTasks(IEnumerable<long> mutationIds)
        {
            var tasks = mutationIds
                .Select(mutationId => new Task
                {
                    TypeId = TaskType.Indexing,
                    TargetTypeId = TaskTargetType.Mutation,
                    Target = mutationId.ToString(),
                    Date = DateTime.UtcNow
                })
                .ToArray();

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.SaveChanges();
        }

        private void CreateDonorIndexingTasks(IEnumerable<long> mutationIds)
        {
            var donorIds = _dbContext.MutationOccurrences
                .Where(mutationOccurrence => mutationIds.Contains(mutationOccurrence.MutationId))
                .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId)
                .Distinct()
                .ToArray();

            var tasks = donorIds
                .Select(donorId => new Task
                {
                    TypeId = TaskType.Indexing,
                    TargetTypeId = TaskTargetType.Donor,
                    Target = donorId.ToString(),
                    Date = DateTime.UtcNow
                })
                .ToArray();

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.SaveChanges();
        }

        private void CreateSpecimenIndexingTasks(IEnumerable<long> mutationIds)
        {
            var specimenIds = _dbContext.MutationOccurrences
                .Where(mutationOccurrence => mutationIds.Contains(mutationOccurrence.MutationId))
                .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.SpecimenId)
                .Distinct()
                .ToArray();

            var tasks = specimenIds
                .Select(specimenId => new Task
                {
                    TypeId = TaskType.Indexing,
                    TargetTypeId = TaskTargetType.Specimen,
                    Target = specimenId.ToString(),
                    Date = DateTime.UtcNow
                })
                .ToArray();

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
