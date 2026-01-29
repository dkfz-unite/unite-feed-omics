using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class AnalysisController(
    SubmissionTaskService submissionTaskService,
    ILogger<AnalysisController> logger)
    : SubmissionController<AnalysisModel<EmptyModel>, AnalysisForm>(submissionTaskService, logger)
{
    protected abstract AnalysisType[] AnalysisTypes { get; }
    
    protected override AnalysisModel<EmptyModel> Convert(AnalysisForm form)
    {
        return AnalysisFormConverter<EmptyModel>.Convert(form);
    }

    //TODO: move validation to s separate class
    protected override void ValidateModel(AnalysisModel<EmptyModel> model)
    {
        if (model.TargetSample != null && !AnalysisTypes.Contains(model.TargetSample.AnalysisType.Value))
        {
            var allowedValues = string.Join(", ", AnalysisTypes.Select(analysis => analysis.ToDefinitionString()));
            ModelState.AddModelError("TargetSample.AnalysisType", $"Allowed values are [{allowedValues}]");
        }

        if (model.MatchedSample != null && !AnalysisTypes.Contains(model.MatchedSample.AnalysisType.Value))
        {
            var allowedValues = string.Join(", ", AnalysisTypes.Select(analysis => analysis.ToDefinitionString()));
            ModelState.AddModelError("MatchedSample.AnalysisType", $"Allowed values are [{allowedValues}]");
        }
    }
}
