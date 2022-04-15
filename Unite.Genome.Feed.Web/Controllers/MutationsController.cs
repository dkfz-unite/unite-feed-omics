using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Data.Extensions;
using Unite.Genome.Feed.Data.Mutations;
using Unite.Genome.Feed.Web.Services;
using Unite.Genome.Feed.Web.Services.Mutations;
using Unite.Genome.Feed.Web.Services.Mutations.Converters;

namespace Unite.Genome.Feed.Web.Controllers
{
    [Route("api/[controller]")]
    public class MutationsController : Controller
    {
        private readonly MutationDataWriter _dataWriter;
        //private readonly MutationIndexingTaskService _indexingTaskService;
        private readonly MutationAnnotationTaskService _annotationTaskService;
        private readonly ILogger _logger;

        private readonly AnalysisModelConverter _converter;

        public MutationsController(
            MutationDataWriter dataWriter,
            //MutationIndexingTaskService indexingTaskService,
            MutationAnnotationTaskService annotationTaskService,
            ILogger<MutationsController> logger)
        {
            _dataWriter = dataWriter;
            //_indexingTaskService = indexingTaskService;
            _annotationTaskService = annotationTaskService;
            _logger = logger;

            _converter = new AnalysisModelConverter();
        }

        [HttpPost]
        public IActionResult Post([FromBody] MutationsModel[] models)
        {
            models.ForEach(model => model.Sanitise());

            var dataModels = models.Select(model => _converter.Convert(model));

            _dataWriter.SaveData(dataModels, out var audit);

            _logger.LogInformation(audit.ToString());

            _annotationTaskService.PopulateTasks(audit.Mutations);
            //_indexingTaskService.PopulateTasks(audit.MutationOccurrences.Where(id => !audit.Mutations.Contains(id)).ToArray());


            return Ok();
        }
    }
}
