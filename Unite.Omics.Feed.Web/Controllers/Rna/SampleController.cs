using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Services.Indexing;

namespace Unite.Omics.Feed.Web.Controllers.Rna;

[Route("api/rna/sample")]
public class SampleController : Controllers.SampleController
{
    protected override string DataType => DataTypes.Omics.Rna.Sample;
    protected override AnalysisType[] AnalysisTypes => [ AnalysisType.RNASeq ];

    public SampleController(SampleWriter dataWriter, SampleIndexingTaskService taskService, ILogger<SampleController> logger) : base(dataWriter, taskService, logger)
    {
    }
}
