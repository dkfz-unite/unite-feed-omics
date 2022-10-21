using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Web.Models.Variants;
using Unite.Genome.Feed.Web.Models.Variants.SSM.Converters;
using Unite.Genome.Feed.Web.Services;

namespace Unite.Genome.Feed.Web.Controllers.Variants;

[Route("api/ssm")]
[ApiController]
public class MutationsController : Controller
{
    private readonly SequencingDataWriter _dataWriter;
    private readonly MutationAnnotationTaskService _annotationTaskService;
    private readonly ILogger _logger;

    private readonly SequencingDataModelConverter _converter;

    public MutationsController(
        SequencingDataWriter dataWriter,
        MutationAnnotationTaskService annotationTaskService,
        ILogger<MutationsController> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _logger = logger;

        _converter = new SequencingDataModelConverter();
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] SequencingDataModel<Models.Variants.SSM.VariantModel>[] models)
    {
        var dataModels = models.Select(model => _converter.Convert(model));

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _annotationTaskService.PopulateTasks(audit.Mutations);

        return Ok();
    }
}
