using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Mutations.DataFeed.Domain.Resources.Mutations;
using Unite.Mutations.DataFeed.Domain.Resources.Mutations.Extensions;
using Unite.Mutations.DataFeed.Domain.Validation;
using Unite.Mutations.DataFeed.Web.Controllers.Extensions;
using Unite.Mutations.DataFeed.Web.Services;

namespace Unite.Mutations.DataFeed.Web.Controllers
{
    [Route("api/[controller]")]
    public class MutationsController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IValidator<IEnumerable<Mutation>> _validator;
        private readonly IDataFeedService _dataFeedService;
        private readonly ILogger _logger;

        public MutationsController(
            IValidationService validationService,
            IValidator<IEnumerable<Mutation>> validator,
            IDataFeedService dataFeedService,
            ILogger<MutationsController> logger)
        {
            _validationService = validationService;
            _validator = validator;
            _dataFeedService = dataFeedService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Mutation[] mutations)
        {
            if (!ModelState.IsValid(out string modelStateErrorMessage))
            {
                _logger.LogWarning(modelStateErrorMessage);

                return BadRequest(modelStateErrorMessage);
            }

            if (!_validationService.ValidateParameter(mutations, _validator, out string modelErrorMessage))
            {
                _logger.LogWarning(modelErrorMessage);

                return BadRequest(modelErrorMessage);
            }

            _dataFeedService.ProcessSamples(mutations.AsSamples());

            return Ok();
        }
    }
}
