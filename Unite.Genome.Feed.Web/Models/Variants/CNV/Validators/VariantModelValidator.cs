using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Validators;

public class VariantModelValidator : AbstractValidator<VariantModel>
{
    public VariantModelValidator()
    {
        RuleFor(model => model.Chromosome)
            .NotEmpty().WithMessage("Should not be empty");

        RuleFor(model => model.Start)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.End)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.Type)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.C1Mean)
            .Must(value => value >= 0)
            .When(model => model.C1Mean != null)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.C2Mean)
            .Must(value => value >= 0)
            .When(model => model.C2Mean != null)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.TcnMean)
            .Must(value => value >= 0)
            .When(model => model.TcnMean != null)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.C1)
            .Must(value => value >= -1)
            .When(model => model.C1 != null)
            .WithMessage("Should be greater than or equal to 0 or -1 if subclonal");

        RuleFor(model => model.C2)
            .Must(value => value >= -1)
            .When(model => model.C2 != null)
            .WithMessage("Should be greater than or equal to 0 or -1 if subclonal");

        RuleFor(model => model.Tcn)
            .Must(value => value >= -1)
            .When(model => model.Tcn != null)
            .WithMessage("Should be greater than or equal to 0 or -1 if subclonal");

        RuleFor(model => model.DhMax)
            .Must(value => value > 0)
            .When(model => model.DhMax != null)
            .WithMessage("Should be greater than or equal to 0");
    }
}
