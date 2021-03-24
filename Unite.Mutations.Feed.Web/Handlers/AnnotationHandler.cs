using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Mutations.Feed.Annotations;
using Unite.Mutations.Feed.Annotations.VEP.Resources;
using Unite.Mutations.Feed.Data.Services;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep.Audit;
using Unite.Mutations.Feed.Data.Services.Mutations;
using Unite.Mutations.Feed.Web.Resources.Annotations.Converters;

namespace Unite.Mutations.Feed.Web.Handlers
{
    public class AnnotationHandler
    {
        private UniteDbContext _dbContext;
        private IAnnotationApiClient<AnnotationResource> _vepAnnotationApiClient;
        private IDataService<AnnotationModel, AnnotationsUploadAudit> _vepAnnotationDataService;
        private MutationIndexingTaskService _mutationIndexingTaskService;
        private readonly ILogger _logger;

        public AnnotationHandler(
            UniteDbContext dbContext,
            IAnnotationApiClient<AnnotationResource> vepAnnotationApiClient,
            IDataService<AnnotationModel, AnnotationsUploadAudit> vepAnnotationDataService,
            MutationIndexingTaskService mutationIndexingTaskService,
            ILogger<AnnotationHandler> logger)
        {
            _dbContext = dbContext;
            _vepAnnotationApiClient = vepAnnotationApiClient;
            _vepAnnotationDataService = vepAnnotationDataService;
            _mutationIndexingTaskService = mutationIndexingTaskService;
            _logger = logger;
        }

        public void Handle(int bucketSize)
        {
            try
            {
                ProcessVepAnnotationTasks(bucketSize);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);

                if(exception.InnerException != null)
                {
                    _logger.LogError(exception.InnerException.Message);
                }
            }
        }


        private void ProcessVepAnnotationTasks(int bucketSize)
        {
            while (_dbContext.Tasks.Any(IsVepMutationAnnotationTask))
            {
                var tasks = _dbContext.Tasks
                    .Where(IsVepMutationAnnotationTask)
                    .OrderByDescending(task => task.Date)
                    .Take(bucketSize)
                    .ToArray();

                var ids = tasks
                    .Select(task => int.Parse(task.Target))
                    .ToArray();

                var hgvsCodes = _dbContext.Mutations
                    .Where(mutation => ids.Any(id => mutation.Id == id))
                    .Select(mutation => mutation.Code)
                    .ToArray();

                _logger.LogInformation($"Annotating {tasks.Length} mutations");

                _logger.LogInformation("Requesting annotations from VEP service");
                var annotationResources = _vepAnnotationApiClient.GetAnnotations(hgvsCodes);

                _logger.LogWarning(JsonSerializer.Serialize(hgvsCodes));

                _logger.LogInformation("Writing annotations to database");
                var annotationModels = annotationResources.Select(resource => AnnotationResourceConverter.From(resource));
                _vepAnnotationDataService.SaveData(annotationModels, out var audit);
                _logger.LogInformation(audit.ToString());

                _logger.LogInformation("Populationg indexing tasks");
                _mutationIndexingTaskService.PopulateTasks(audit.Mutations);

                _dbContext.Tasks.RemoveRange(tasks);
                _dbContext.SaveChanges();

                _logger.LogInformation($"Annotation of {tasks.Length} mutations completed");
            }
        }

        private bool IsVepMutationAnnotationTask(Task task)
        {
            return task.TypeId == TaskType.AnnotationVEP && task.TargetTypeId == TaskTargetType.Mutation;
        }
    }
}
