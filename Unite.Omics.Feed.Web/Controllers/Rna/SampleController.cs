using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Submissions.Repositories.Rna;

namespace Unite.Omics.Feed.Web.Controllers.Rna;

[Route("api/rna/sample")]
public class SampleController : Controllers.SampleController
{
    public SampleController(SubmissionTaskService submissionTaskService,
        ILogger<SampleController> logger,
        SampleSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNA;
    protected override string DataType => DataTypes.Omics.Rna.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.RNASeq];
}
