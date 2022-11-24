using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Validators;

public class VariantAceSeqModelValidator : AbstractValidator<VariantAceSeqModel>
{
    public VariantAceSeqModelValidator()
    {
        RuleFor(model => model.Chromosome)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Start)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.End)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.SvType)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.CnaType)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.GetC1Mean())
            .Must(value => value >= 0)
            .When(model => model.GetC1Mean().HasValue)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.GetC2Mean())
           .Must(value => value >= 0)
           .When(model => model.GetC2Mean().HasValue)
           .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.GetTcnMean())
           .Must(value => value >= 0)
           .When(model => model.GetTcnMean().HasValue)
           .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.GetC1())
            .Must(value => value >= -1)
            .When(model => model.GetC1().HasValue)
            .WithMessage("Should be greater than or equal to 0 or 'sub'");

        RuleFor(model => model.GetC2())
            .Must(value => value >= -1)
            .When(model => model.GetC2().HasValue)
            .WithMessage("Should be greater than or equal to 0 or 'sub'");

        RuleFor(model => model.GetTcn())
            .Must(value => value >= -1)
            .When(model => model.GetTcn().HasValue)
            .WithMessage("Should be greater than or equal to 0 or 'sub'");

        RuleFor(model => model.GetDhMax())
           .Must(value => value >= 0)
           .When(model => model.GetDhMax().HasValue)
           .WithMessage("Should be greater than or equal to 0");
    }
}
