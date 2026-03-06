using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

namespace Unite.Omics.Feed.Web.Controllers.Meth;

[Route("api/meth/analysis/levels")]
[Authorize(Policy = Policies.Data.Writer)]
public class LevelsController : AnalysisController<EmptyModel>
{
    public LevelsController(SubmissionTaskService submissionTaskService,
        ILogger<LevelsController> logger,
        LevelSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.METH_LVL;
    protected override string DataType => DataTypes.Omics.Meth.Level;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.MethArray, AnalysisType.WGBS, AnalysisType.RRBS];
}
