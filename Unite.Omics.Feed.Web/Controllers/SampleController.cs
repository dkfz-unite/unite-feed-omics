using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Binders;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Services.Indexing;

namespace Unite.Omics.Feed.Web.Controllers;

[Authorize(Policy = Policies.Data.Writer)]
public abstract class SampleController : Controller
{
    private readonly SampleWriter _dataWriter;
    private readonly SampleIndexingTaskService _taskService;
    private readonly ILogger _logger;

    private readonly SampleModelConverter _converter = new();
    private readonly IValidator<ResourceModel> _resourceModelValidator = new ResourceModelValidator();


    public SampleController(
        SampleWriter dataWriter,
        SampleIndexingTaskService taskService,
        ILogger<SampleController> logger)
    {
        _dataWriter = dataWriter;
        _taskService = taskService;
        _logger = logger;
    }


    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok();
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] SampleModel model, [FromQuery] bool review = true)
    {
        // TODO: Add submission process
        var dataModel = _converter.Convert(model);

        _dataWriter.SaveData(dataModel, out var audit);

        _taskService.PopulateTasks(audit.Samples);

        _logger.LogInformation("{audit}", audit.ToString());

        return Ok();
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(SampleTsvModelBinder))] SampleModel model, [FromQuery] bool review = true)
    {
        return TryValidateModel(model) ? Post(model, review) : BadRequest(ModelState);
    }

    [HttpPost("file")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostFile([FromForm] SampleForm form, [FromQuery] bool review = true)
    {
        var model = form.Convert();

        if (!form.ResourcesFile.IsEmpty())
        {
            model.Resources = ParseResources(form.ResourcesFile);
            ValidateItems(model.Resources, _resourceModelValidator, "Resources");
        }

        return Post(model, review);
    }


    protected virtual ResourceModel[] ParseResources(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var streamReader = new StreamReader(stream);

        var tsv = streamReader.ReadToEnd();

        return TsvReader.Read<ResourceModel>(tsv).ToArrayOrNull();
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
