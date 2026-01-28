using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile.Validators;
using Unite.Omics.Feed.Web.Submissions;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/cnv-profile")]
[Authorize(Policy = Policies.Data.Writer)]
public class CnvProfileController: AnalysisDataController<CnvProfileModel>
{
    private readonly CnvProfileSubmissionRepository _submissionRepository;
    
    protected override IValidator<CnvProfileModel> EntryModelValidator => new CnvProfileModelValidator();
    protected override string DataType => DataTypes.Omics.Dna.CnvProfile;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];
    protected override IReader<CnvProfileModel>[] Readers => 
    [
        new TsvReader<CnvProfileModel>()
    ];

    //TODO: change to CNV_PROFILE task type
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_SM;
    protected override SubmissionRepository SubmissionRepository => _submissionRepository;

    public CnvProfileController(SubmissionTaskService submissionTaskService, 
        ILogger<AnalysisDataController<CnvProfileModel>> logger, 
        CnvProfileSubmissionRepository submissionRepository) : base(submissionTaskService, logger)
    {
        _submissionRepository = submissionRepository;
    }
    
    protected override AnalysisModel<CnvProfileModel> FindSubmission(string id)
    {
        throw new NotImplementedException();
    }
}