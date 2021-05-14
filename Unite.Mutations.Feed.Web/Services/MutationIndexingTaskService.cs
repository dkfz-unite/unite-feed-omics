using System;
using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Web.Services
{
    public class MutationIndexingTaskService
    {
        private readonly UniteDbContext _dbContext;


        public MutationIndexingTaskService(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void PopulateTasks(IEnumerable<long> mutationIds)
        {
            PopulateMutationIndexingTasks(mutationIds);
            PopulateDonorIndexingTasks(mutationIds);
            //PopulateSpecimenIndexingTasks(mutationIds);
        }


        private void PopulateMutationIndexingTasks(IEnumerable<long> mutationIds)
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

        private void PopulateDonorIndexingTasks(IEnumerable<long> mutationIds)
        {
            var donorIds = _dbContext.MutationOccurrences
                .Where(mutationOccurrence =>
                    mutationIds.Contains(mutationOccurrence.MutationId) &&
                    mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId != null
                )
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

        private void PopulateSpecimenIndexingTasks(IEnumerable<long> mutationIds)
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
    }
}
