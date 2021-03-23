using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Mutations.Feed.Data.Services;
using Unite.Mutations.Feed.Data.Services.Mutations.Models;
using Unite.Mutations.Feed.Data.Services.Mutations.Models.Audit;
using Unite.Mutations.Feed.Web.Resources.Extensions;
using Unite.Mutations.Feed.Web.Resources.Mutations;
using Unite.Mutations.Feed.Web.Resources.Mutations.Converters;
using Unite.Mutations.Feed.Web.Resources.Validation;

namespace Unite.Mutations.Feed.Web.Controllers
{
    [Route("api/[controller]")]
    public class MutationsController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IValidator<IEnumerable<MutationsResource>> _validator;
        private readonly IDataService<MutationsModel, MutationsUploadAudit> _dataService;
        private readonly VepAnnotationTaskService _annotationTaskService;
        private readonly ILogger _logger;

        public MutationsController(
            IValidationService validationService,
            IValidator<IEnumerable<MutationsResource>> validator,
            IDataService<MutationsModel, MutationsUploadAudit> dataService,
            VepAnnotationTaskService annotationTaskService,
            ILogger<MutationsController> logger)
        {
            _validationService = validationService;
            _validator = validator;
            _dataService = dataService;
            _annotationTaskService = annotationTaskService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] MutationsResource[] resources)
        {
            if (!_validationService.ValidateParameter(resources, _validator, out string modelErrorMessage))
            {
                _logger.LogWarning(modelErrorMessage);

                return BadRequest(modelErrorMessage);
            }

            _logger.LogInformation("Processing mutations");

            _logger.LogInformation("Sanitising data");
            resources.ForEach(resource => resource.Sanitise());

            _logger.LogInformation("Writing data to database");
            var models = resources.Select(resource => MutationsResourceConverter.From(resource));
            _dataService.SaveData(models, out var audit);
            _logger.LogInformation(audit.ToString());

            _logger.LogInformation("Populating annotation tasks");
            _annotationTaskService.PopulateTasks(audit.Mutations);

            _logger.LogInformation("Processing mutations completed");

            return Ok();
        }
    }
}
