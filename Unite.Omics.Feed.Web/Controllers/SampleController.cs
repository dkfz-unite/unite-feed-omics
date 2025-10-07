using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Validators;

namespace Unite.Omics.Feed.Web.Controllers;

[Authorize(Policy = Policies.Data.Writer)]
public abstract class SampleController : Controller
{
    protected readonly SubmissionTaskService _submissionTaskService;
    protected readonly ILogger _logger;

    protected readonly SampleModelConverter _converter = new();
    protected readonly IValidator<ResourceModel> _resourceModelValidator = new ResourceModelValidator();

    protected abstract string DataType { get; }
    protected abstract AnalysisType[] AnalysisTypes { get; }


    public SampleController(
        SubmissionTaskService submissionTaskService,
        ILogger<SampleController> logger)
    {
        _submissionTaskService = submissionTaskService;
        _logger = logger;
    }


    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult Get(long id)
    {
        var submission = GetSubmission(id);

        return Ok(submission);
    }

    [HttpPost("")]
    [Consumes("application/json")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostJson([FromBody] SampleModel model, [FromQuery] bool review = true)
    {
        model.Resources?.ForEach(resource => resource.Type = DataType);
        ValidateSample(model);

        return ModelState.IsValid ? Ok(AddSubmission(model, review)) : BadRequest(ModelState);
    }

    [HttpPost("")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostForm([FromForm] SampleForm form, [FromQuery] bool review = true)
    {
        var model = form.Convert();
        ValidateSample(model);

        if (!form.ResourcesFile.IsEmpty())
        {
            model.Resources = ParseResources(form.ResourcesFile);
            model.Resources?.ForEach(resource => resource.Type = DataType);
            ValidateResources(model.Resources);
        }

        return ModelState.IsValid ? Ok(AddSubmission(model, review)) : BadRequest(ModelState);
    }


    protected abstract SampleModel GetSubmission(long id);

    protected abstract long AddSubmission(SampleModel model, bool review);


    protected virtual ResourceModel[] ParseResources(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var streamReader = new StreamReader(stream);

        var tsv = streamReader.ReadToEnd();

        return TsvReader.Read<ResourceModel>(tsv).ToArrayOrNull();
    }

    protected virtual void ValidateSample(SampleModel model)
    {
        if (!AnalysisTypes.Contains(model.AnalysisType.Value))
        {
            var allowedValues = string.Join(", ", AnalysisTypes.Select(analysis => analysis.ToDefinitionString()));
            ModelState.AddModelError("AnalysisType", $"Allowed values are [{allowedValues}]");
        }
    }

    protected virtual void ValidateResources(ResourceModel[] resources)
    {
        ValidateItems(resources, _resourceModelValidator, "Resources");
    }

    protected virtual void ValidateItems<T>(T[] items, IValidator<T> validator, string prefix)
    {
        if (items.IsEmpty())
        {
            ModelState.AddModelError(prefix, "Should not be empty");
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            var result = validator.Validate(items[i]);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError($"{prefix}[{i}].{error.PropertyName}", error.ErrorMessage);
                }
            }
        }
    }
}
