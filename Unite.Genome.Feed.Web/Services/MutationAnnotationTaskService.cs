using System;
using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Web.Services
{
    public class MutationAnnotationTaskService : AnnotationTaskService<Mutation, long>
    {
        protected override int BucketSize => 1000;


        public MutationAnnotationTaskService(DomainDbContext dbContext) : base(dbContext)
        {
        }


        /// <summary>
        /// Creates only mutation annotation tasks for all existing mutations
        /// </summary>
        public override void CreateTasks()
        {
            IterateEntities<Mutation, long>(mutation => true, mutation => mutation.Id, mutations =>
            {
                CreateMutationAnnotationTasks(mutations);
            });
        }

        /// <summary>
        /// Creates only mutation annotation tasks for all mutations with given identifiers
        /// </summary>
        /// <param name="mutationIds">Identifiers of mutations</param>
        public override void CreateTasks(IEnumerable<long> mutationIds)
        {
            IterateEntities<Mutation, long>(mutation => mutationIds.Contains(mutation.Id), mutation => mutation.Id, mutations =>
            {
                CreateMutationAnnotationTasks(mutations);
            });
        }

        /// <summary>
        /// Populates all types of annotation tasks for mutations with given identifiers
        /// </summary>
        /// <param name="mutationIds">Identifiers of mutations</param>
        public override void PopulateTasks(IEnumerable<long> mutationIds)
        {
            IterateEntities<Mutation, long>(mutation => mutationIds.Contains(mutation.Id), mutation => mutation.Id, mutations =>
            {
                CreateMutationAnnotationTasks(mutations);
            });
        }


        private void CreateMutationAnnotationTasks(IEnumerable<long> mutationIds)
        {
            CreateTasks(TaskType.Annotation, TaskTargetType.Mutation, mutationIds);
        }
    }
}
