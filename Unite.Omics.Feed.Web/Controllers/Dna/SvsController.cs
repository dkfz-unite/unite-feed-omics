using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Dna.Sv;
using Unite.Omics.Feed.Web.Models.Dna.Sv.Validators;
using Unite.Omics.Feed.Web.Submissions;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sv")]
[Authorize(Policy = Policies.Data.Writer)]
public class SvsController : AnalysisDataController<VariantModel>
{
    private readonly DnaSubmissionService _submissionService;
    private readonly SvSubmissionRepository _submissionRepository;

    protected override IValidator<VariantModel> EntryModelValidator => new VariantModelValidator();
    protected override string DataType => DataTypes.Omics.Dna.Sv;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];
    protected override IReader<VariantModel>[] Readers =>
    [
        new TsvReader<VariantModel>(),
        new Models.Dna.Sv.Readers.DkfzSophia.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_SV;
    protected override SubmissionRepository SubmissionRepository => _submissionRepository;

    public SvsController(
        DnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService,
        ILogger<SvsController> logger, 
        SvSubmissionRepository submissionRepository) : base(submissionTaskService, logger)
    {
        _submissionService = submissionService;
        _submissionRepository = submissionRepository;
    }


    protected override AnalysisModel<VariantModel> FindSubmission(string id)
    {
        return _submissionService.FindSvSubmission(id);
    }
}
