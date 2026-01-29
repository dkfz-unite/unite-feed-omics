using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Binders;
using Unite.Omics.Feed.Web.Models.Base.Converters;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class SampleController(
    SubmissionTaskService submissionTaskService,
    ILogger<SampleController> logger)
    : SubmissionController<SampleModel, SampleForm>(submissionTaskService, logger)
{
    protected readonly SampleModelConverter _converter = new();
    
    protected abstract AnalysisType[] AnalysisTypes { get; }

    protected override SampleModel Convert(SampleForm form)
    {
        return new SampleModel
        {
            DonorId = form.DonorId,
            SpecimenId = form.SpecimenId,
            SpecimenType = EnumBinder.Bind<SpecimenType>(form.SpecimenType).Value,
            AnalysisType = EnumBinder.Bind<AnalysisType>(form.AnalysisType).Value,
            AnalysisDate = form.AnalysisDate,
            AnalysisDay = form.AnalysisDay,
            Genome = form.Genome
        };
    }

    protected override void ValidateModel(SampleModel model)
    {
        if (!AnalysisTypes.Contains(model.AnalysisType.Value))
        {
            var allowedValues = string.Join(", ", AnalysisTypes.Select(analysis => analysis.ToDefinitionString()));
            ModelState.AddModelError("AnalysisType", $"Allowed values are [{allowedValues}]");
        }
    }
}
