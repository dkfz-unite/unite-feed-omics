using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/sample")]
public class SampleController : SubmissionController<SampleModel, SampleForm, EmptyModel>
{
    protected override string DataType => DataTypes.Omics.Dna.Sample;
    protected override IValidator<SampleModel> ModelValidator => new SampleModelValidator([AnalysisType.WGS, AnalysisType.WES]);
    protected override IFormConverter<SampleModel, SampleForm, EmptyModel> FormConverter => new SampleFormConverter();
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA;


    public SampleController(SubmissionTaskService submissionTaskService,
        ILogger<SampleController> logger,
        Unite.Omics.Feed.Web.Submissions.Repositories.Dna.SampleSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
