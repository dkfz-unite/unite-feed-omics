using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Web.Models.Variants;
using Unite.Genome.Feed.Web.Models.Variants.CNV.Converters;
using Unite.Genome.Feed.Web.Services;

namespace Unite.Genome.Feed.Web.Controllers.Variants;

[Route("api/cnv")]
[ApiController]
public class CopyNumberVariantsController : Controller
{
    private readonly SequencingDataWriter _dataWriter;
    private readonly CopyNumberVariantAnnotationTaskService _annotationTaskService;
    private readonly ILogger _logger;

    private readonly SequencingDataModelConverter _standardConverter;
    private readonly SequencingDataAceSeqModelConverter _aceSeqConverter;

    public CopyNumberVariantsController(
        SequencingDataWriter dataWriter,
        CopyNumberVariantAnnotationTaskService annotationTaskService,
        ILogger<MutationsController> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _logger = logger;

        _standardConverter = new SequencingDataModelConverter();
        _aceSeqConverter = new SequencingDataAceSeqModelConverter();
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] SequencingDataModel<Models.Variants.CNV.VariantModel>[] models)
    {
        var dataModels = models.Select(model => _standardConverter.Convert(model));

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _annotationTaskService.PopulateTasks(audit.CopyNumberVariants);

        return Ok();
    }

    [HttpPost("aceseq")]
    public IActionResult Post([FromBody] SequencingDataModel<Models.Variants.CNV.VariantAceSeqModel>[] models)
    {
        var dataModels = models.Select(model => _aceSeqConverter.Convert(model));

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _annotationTaskService.PopulateTasks(audit.CopyNumberVariants);

        return Ok();
    }
}
