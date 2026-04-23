using FluentValidation;

namespace Unite.Omics.Feed.Web.Models.Dna.CnvProfile.Validators;

public class CnvProfileModelValidator:  AbstractValidator<CnvProfileModel>
{
    public CnvProfileModelValidator()
    {
        RuleFor(model => model.Chromosome)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.ChromosomeArm)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Gain)
            .InclusiveBetween(0, 1)
            .WithMessage("Should be in range [0, 1]");
        
        RuleFor(model => model.Neutral)
            .InclusiveBetween(0, 1)
            .WithMessage("Should be in range [0, 1]");

        RuleFor(model => model.Loss)
            .InclusiveBetween(0, 1)
            .WithMessage("Should be in range [0, 1]");

        RuleFor(model => model)
            .Must(model => model.Gain > 0 || model.Neutral > 0 || model.Loss > 0)
            .WithMessage("At least one of 'gain', 'neutral' or 'loss' should be greater than 0");
    }
}
