using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class AnalysisController<TEntry> : SubmissionController<AnalysisModel<TEntry>, AnalysisForm> 
        where TEntry : class, new()
{
    protected AnalysisController(SubmissionTaskService submissionTaskService,
        SubmissionRepository<AnalysisModel<TEntry>> submissionRepository,
        ILogger<AnalysisController<TEntry>> logger) : base(submissionTaskService, submissionRepository, logger)
    {
    }

    protected abstract AnalysisType[] AnalysisTypes { get; }
    
    protected override AnalysisModel<TEntry> Convert(AnalysisForm form)
    {
        return AnalysisFormConverter<TEntry>.Convert(form);
    }
    
    protected override void ValidateModel(AnalysisModel<TEntry> model)
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
