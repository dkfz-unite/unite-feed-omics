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

        RuleFor(model => model.C1Mean)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.C2Mean)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.TcnMean)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");

        RuleFor(model => model.C1)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(IsIntegerOrSub).WithMessage("Should be greater than or equal to 0 or 'sub'");

        RuleFor(model => model.C2)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(IsIntegerOrSub).WithMessage("Should be greater than or equal to 0 or 'sub'");

        RuleFor(model => model.Tcn)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(IsIntegerOrSub).WithMessage("Should be greater than or equal to 0 or 'sub'");

        RuleFor(model => model.DhMax)
            .NotEmpty().WithMessage("Should not be empty")
            .Must(value => value > 0).WithMessage("Should be greater than 0");
    }


    private static bool IsIntegerOrSub(string value)
    {
        return string.Equals(value, "sub", StringComparison.InvariantCultureIgnoreCase) || int.TryParse(value, out _);
    }
}
