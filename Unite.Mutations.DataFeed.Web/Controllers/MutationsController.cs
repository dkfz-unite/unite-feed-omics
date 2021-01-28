using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Mutations.DataFeed.Domain.Resources.Mutations;
using Unite.Mutations.DataFeed.Domain.Validation;
using Unite.Mutations.DataFeed.Web.Controllers.Extensions;
using Unite.Mutations.DataFeed.Web.Services;

namespace Unite.Mutations.DataFeed.Web.Controllers
{
    [Route("api/[controller]")]
    public class MutationsController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IValidator<IEnumerable<Resource>> _validator;
        private readonly IDataFeedService<Resource> _dataFeedService;
        private readonly ILogger _logger;

        public MutationsController(
            IValidationService validationService,
            IValidator<IEnumerable<Resource>> validator,
            IDataFeedService<Resource> dataFeedService,
            ILogger<MutationsController> logger)
        {
            _validationService = validationService;
            _validator = validator;
            _dataFeedService = dataFeedService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Resource[] resources)
        {
            if (!ModelState.IsValid(out string modelStateErrorMessage))
            {
                _logger.LogWarning(modelStateErrorMessage);

                return BadRequest(modelStateErrorMessage);
            }

            if (!_validationService.ValidateParameter(resources, _validator, out string modelErrorMessage))
            {
                _logger.LogWarning(modelErrorMessage);

                return BadRequest(modelErrorMessage);
            }

            _dataFeedService.ProcessResources(resources);

            return Ok();
        }
    }
}
