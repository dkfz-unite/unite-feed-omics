using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/sample")]
public class SampleController : Controllers.SampleController
{
    private readonly Unite.Omics.Feed.Web.Submissions.Repositories.Dna.SampleSubmissionRepository _submissionRepository;
    protected override string DataType => DataTypes.Omics.Dna.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA;
    protected override SubmissionRepository SubmissionRepository => _submissionRepository;


    public SampleController(SubmissionTaskService submissionTaskService, 
        ILogger<SampleController> logger, 
        Unite.Omics.Feed.Web.Submissions.Repositories.Dna.SampleSubmissionRepository submissionRepository) : base(submissionTaskService, logger)
    {
        _submissionRepository = submissionRepository;
    }
}
