using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Data.Writers;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Base.Binders;
using Unite.Genome.Feed.Web.Models.Base.Converters;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Controllers;

[Authorize(Policy = Policies.Data.Writer)]
public abstract class SampleController : Controller
{
    private readonly SampleWriter _dataWriter;
    private readonly SampleIndexingTaskService _taskService;
    private readonly ILogger _logger;

    private readonly SampleModelConverter _converter = new();


    public SampleController(
        SampleWriter dataWriter,
        SampleIndexingTaskService taskService,
        ILogger<SampleController> logger)
    {
        _dataWriter = dataWriter;
        _taskService = taskService;
        _logger = logger;
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
        return Post(model, review);
    }
}
