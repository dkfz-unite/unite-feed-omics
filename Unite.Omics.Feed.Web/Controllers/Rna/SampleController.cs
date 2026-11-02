using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.Rna;

namespace Unite.Omics.Feed.Web.Controllers.Rna;

[Route("api/rna/sample")]
public class SampleController : SubmissionController<SampleModel, SampleForm, EmptyModel>
{
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNA;
    protected override string DataType => DataTypes.Omics.Rna.Sample;
    protected override IValidator<SampleModel> ModelValidator => new SampleModelValidator([AnalysisType.RNASeq]);
    protected override IFormConverter<SampleModel, SampleForm, EmptyModel> FormConverter => new SampleFormConverter();


    public SampleController(SubmissionTaskService submissionTaskService,
        ILogger<SampleController> logger,
        SampleSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
