using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;
using Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/sample")]
public class SampleController(
    SubmissionTaskService submissionTaskService,
    ILogger<SampleController> logger,
    SampleSubmissionRepository submissionRepository)
    : Controllers.SampleController(submissionTaskService, logger)
{
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNASC;
    protected override SubmissionRepository SubmissionRepository => submissionRepository;
    protected override string DataType => DataTypes.Omics.Rnasc.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.RNASeqSc, AnalysisType.RNASeqSn];
}
