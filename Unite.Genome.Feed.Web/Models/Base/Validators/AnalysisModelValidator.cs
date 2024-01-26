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

        RuleFor(model => model.Date)
            .Empty()
            .When(model => model.Day.HasValue)
            .WithMessage("Either exact 'date' or relative 'days' can be set, not both");

        RuleFor(model => model.Day)
            .Empty()
            .When(model => model.Date.HasValue)
            .WithMessage("Either exact 'date' or relative 'days' can be set, not both");

        RuleFor(model => model.Day)
            .GreaterThanOrEqualTo(1)
            .When(model => model.Day.HasValue)
            .WithMessage("Should be greater than or equal to 1");
    }
}
