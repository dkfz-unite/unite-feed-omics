using FluentValidation;
using Unite.Data.Entities.Omics.Analysis.Enums;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class AnalysisResourcesModelValidator : AnalysisModelValidator<AnalysisModel<EmptyModel>>
{
    public AnalysisResourcesModelValidator(AnalysisType[] assayTypes) : base(assayTypes)
    {
        RuleFor(model => model.EntriesScope)
            .Empty().WithMessage("Entries scope is not supported for resource submissions");

        RuleFor(model => model.Resources)
            .NotEmpty().WithMessage("Should contain at least one resource");
    }
}
