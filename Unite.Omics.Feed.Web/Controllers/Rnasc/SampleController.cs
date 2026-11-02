using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/sample")]
public class SampleController : SubmissionController<SampleModel, SampleForm, EmptyModel>
{
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNASC;
    protected override string DataType => DataTypes.Omics.Rnasc.Sample;
    protected override IValidator<SampleModel> ModelValidator => new SampleModelValidator([AnalysisType.RNASeqSc, AnalysisType.RNASeqSn]);
    protected override IFormConverter<SampleModel, SampleForm, EmptyModel> FormConverter => new SampleFormConverter();


    public SampleController(SubmissionTaskService submissionTaskService,
        ILogger<SampleController> logger,
        SampleSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
