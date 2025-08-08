using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Services.Indexing;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/sample")]
public class SampleController : Controllers.SampleController
{
    protected override string DataType => DataTypes.Omics.Dna.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];

    public SampleController(SampleWriter dataWriter, SampleIndexingTaskService taskService, ILogger<SampleController> logger) : base(dataWriter, taskService, logger)
    {
    }
}
