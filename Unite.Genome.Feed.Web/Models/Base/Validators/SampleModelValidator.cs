using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class SampleModelValidator : AbstractValidator<SampleModel> 
{
    public SampleModelValidator()
    {
        RuleFor(model => model.DonorId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.DonorId)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");


        RuleFor(model => model.SpecimenId)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.SpecimenId)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");


        RuleFor(model => model.SpecimenType)
            .NotEmpty()
            .WithMessage("Should not be empty");

        
        RuleFor(model => model.Purity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Should be greater than or equal to 0");

        RuleFor(model => model.Purity)
            .LessThanOrEqualTo(1)
            .WithMessage("Should be less than or equal to 1");

        
        RuleFor(model => model.Ploidy)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Should be greater than or equal to 0");


        RuleFor(model => model.CellsNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Should be greater than or equal to 0");


        RuleFor(model => model.GenesModel)
            .NotEmpty()
            .When(model => model.CellsNumber > 0)
            .WithMessage("Should not be empty if 'cellsNumber' is set");

        RuleFor(model => model.GenesModel)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");
    }
}
