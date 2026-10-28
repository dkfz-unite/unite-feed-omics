using FluentValidation;
using Unite.Data.Entities.Omics.Analysis.Enums;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class AnalysisDataModelValidator<TEntry> : AnalysisModelValidator<AnalysisModel<TEntry>> where TEntry : class, new()
{
    public AnalysisDataModelValidator(AnalysisType[] analysisTypes, IValidator<TEntry> entryValidator, string[] scope = null)
        : base(analysisTypes, scope)
    {
        RuleFor(model => model.Entries)
            .NotEmpty().WithMessage("Should contain at least one entry");

        RuleForEach(model => model.Entries)
            .NotNull().WithMessage("Should not be empty")
            .SetValidator(entryValidator);
    }
}
