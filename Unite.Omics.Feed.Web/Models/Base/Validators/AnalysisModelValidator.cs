using FluentValidation;
using Unite.Data.Entities.Omics.Analysis.Enums;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public abstract class AnalysisModelValidator<TModel> : AbstractValidator<TModel> where TModel : AnalysisModel
{
    protected AnalysisModelValidator(AnalysisType[] analysisTypes, string[] scope = null)
    {
        RuleFor(model => model.TargetSample)
            .NotNull().WithMessage("Should not be empty")
            .SetValidator(new SampleModelValidator(analysisTypes, scope));

        RuleFor(model => model.MatchedSample)
            .SetValidator(new SampleModelValidator(analysisTypes));

        RuleForEach(model => model.Resources)
            .NotNull().WithMessage("Should not be empty")
            .SetValidator(new ResourceModelValidator());
    }
}
