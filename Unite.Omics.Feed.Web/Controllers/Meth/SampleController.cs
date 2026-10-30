using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

namespace Unite.Omics.Feed.Web.Controllers.Meth;

[Route("api/meth/sample")]
public class SampleController : SubmissionController<SampleModel, SampleForm, EmptyModel>
{
    protected override string DataType => DataTypes.Omics.Methylation.Sample;
    protected override IValidator<SampleModel> ModelValidator => new Models.Meth.Validators.SampleModelValidator();
    protected override IFormConverter<SampleModel, SampleForm, EmptyModel> FormConverter => new SampleFormConverter();
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.METH;


    public SampleController(SubmissionTaskService submissionTaskService,
        ILogger<SampleController> logger,
        SampleSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
