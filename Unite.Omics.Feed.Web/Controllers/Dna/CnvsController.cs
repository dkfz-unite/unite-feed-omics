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
using Unite.Omics.Feed.Web.Models.Dna.Cnv;
using Unite.Omics.Feed.Web.Models.Dna.Cnv.Validators;
using Unite.Omics.Feed.Web.Submissions;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/cnv")]
[Authorize(Policy = Policies.Data.Writer)]
public class CnvsController : AnalysisDataController<VariantModel>
{
    private readonly CnvSubmissionRepository _submissionRepository;

    protected override IValidator<VariantModel> EntryModelValidator => new VariantModelValidator();
    protected override string DataType => DataTypes.Omics.Dna.Cnv;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];
    protected override IReader<VariantModel>[] Readers =>
    [
        new TsvReader<VariantModel>(),
        new Models.Dna.Cnv.Readers.Aceseq.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_CNV;
    protected override SubmissionRepository SubmissionRepository => _submissionRepository;


    public CnvsController(
        SubmissionTaskService submissionTaskService,
        ILogger<CnvsController> logger, 
        CnvSubmissionRepository submissionRepository) : base(submissionTaskService, logger)
    {
        _submissionRepository = submissionRepository;
    }
}
