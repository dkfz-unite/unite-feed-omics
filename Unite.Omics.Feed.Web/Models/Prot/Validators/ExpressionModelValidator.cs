using FluentValidation;

namespace Unite.Omics.Feed.Web.Models.Prot.Validators;

public class ExpressionModelValidator : AbstractValidator<ExpressionModel>
{
    public ExpressionModelValidator()
    {
        RuleFor(model => model.Id)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Accession)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Symbol)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Intensity)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Intensity)
            .Must(value => value >= 0)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model)
            .Must(model => !string.IsNullOrEmpty(model.Id) || !string.IsNullOrEmpty(model.Accession) || !string.IsNullOrEmpty(model.Symbol))
            .WithMessage("At least one of Id, Accession, or Symbol must be set");
    }
}
