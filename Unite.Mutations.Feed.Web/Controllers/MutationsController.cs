using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Mutations.Feed.Data.Mutations;
using Unite.Mutations.Feed.Web.Models.Extensions;
using Unite.Mutations.Feed.Web.Models.Mutations;
using Unite.Mutations.Feed.Web.Models.Mutations.Converters;
using Unite.Mutations.Feed.Web.Models.Validation;
using Unite.Mutations.Feed.Web.Services;

namespace Unite.Mutations.Feed.Web.Controllers
{
    [Route("api/[controller]")]
    public class MutationsController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IValidator<IEnumerable<MutationsModel>> _validator;
        private readonly MutationDataWriter _dataWriter;
        private readonly MutationIndexingTaskService _indexingTaskService;
        private readonly MutationAnnotationTaskService _annotationTaskService;
        private readonly ILogger _logger;

        private readonly AnalysisModelConverter _converter;

        public MutationsController(
            IValidationService validationService,
            IValidator<IEnumerable<MutationsModel>> validator,
            MutationDataWriter dataWriter,
            MutationIndexingTaskService indexingTaskService,
            MutationAnnotationTaskService annotationTaskService,
            ILogger<MutationsController> logger)
        {
            _validationService = validationService;
            _validator = validator;
            _dataWriter = dataWriter;
            _indexingTaskService = indexingTaskService;
            _annotationTaskService = annotationTaskService;
            _logger = logger;

            _converter = new AnalysisModelConverter();
        }

        [HttpPost]
        public IActionResult Post([FromBody] MutationsModel[] models)
        {
            if (!_validationService.ValidateParameter(models, _validator, out string modelErrorMessage))
            {
                _logger.LogWarning(modelErrorMessage);

                return BadRequest(modelErrorMessage);
            }

            models.ForEach(model => model.Sanitise());

            var dataModels = models.Select(model => _converter.Convert(model));

            _dataWriter.SaveData(dataModels, out var audit);

            _logger.LogInformation(audit.ToString());

            _annotationTaskService.PopulateTasks(audit.Mutations);
            _indexingTaskService.PopulateTasks(audit.MutationOccurrences.Where(id => !audit.Mutations.Contains(id)).ToArray());


            return Ok();
        }
    }
}
