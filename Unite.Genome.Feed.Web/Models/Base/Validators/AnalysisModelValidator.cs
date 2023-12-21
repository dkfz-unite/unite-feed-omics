using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class AnalysisModelValidator : AbstractValidator<AnalysisModel>
{
    public AnalysisModelValidator()
    {
        RuleFor(model => model.Id)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.Type)
            .NotEmpty()
            .WithMessage("Should not be empty");
    }
}
