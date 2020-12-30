using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Mutations.DataFeed.Domain.Resources.Samples;
using Unite.Mutations.DataFeed.Domain.Validation;
using Unite.Mutations.DataFeed.Web.Controllers.Extensions;
using Unite.Mutations.DataFeed.Web.Services;

namespace Unite.Mutations.DataFeed.Web.Controllers
{
    [Route("api/[controller]")]
    public class SamplesController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IValidator<IEnumerable<Sample>> _validator;
        private readonly IDataFeedService _dataFeedService;
        private readonly ILogger _logger;

        public SamplesController(
            IValidationService validationService,
            IValidator<IEnumerable<Sample>> validator,
            IDataFeedService dataFeedService,
            ILogger<MutationsController> logger)
        {
            _validationService = validationService;
            _validator = validator;
            _dataFeedService = dataFeedService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Sample[] samples)
        {
            if (!ModelState.IsValid(out string modelStateErrorMessage))
            {
                _logger.LogWarning(modelStateErrorMessage);

                return BadRequest(modelStateErrorMessage);
            }

            if (!_validationService.ValidateParameter(samples, _validator, out string modelErrorMessage))
            {
                _logger.LogWarning(modelErrorMessage);

                return BadRequest(modelErrorMessage);
            }

            _dataFeedService.ProcessSamples(samples);

            return Ok();
        }
    }
}
