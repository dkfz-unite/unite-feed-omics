using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class SampleModelValidator : AbstractValidator<SampleModel> 
{
    private readonly IValidator<ResourceModel> _resourceModelValidator = new ResourceModelValidator();

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

        
        RuleFor(model => model.AnalysisType)
            .NotEmpty()
            .WithMessage("Should not be empty");


        RuleFor(model => model.AnalysisDate)
            .Empty()
            .When(model => model.AnalysisDay != null)
            .WithMessage("Either exact 'analysis_date' or relative 'analysis_day' can be set, not both");


        RuleFor(model => model.AnalysisDay)
            .Empty()
            .When(model => model.AnalysisDate != null)
            .WithMessage("Either exact 'analysis_date' or relative 'analysis_day' can be set, not both");

        RuleFor(model => model.AnalysisDay)
            .GreaterThanOrEqualTo(1)
            .When(model => model.AnalysisDay != null)
            .WithMessage("Should be greater than or equal to 1");

        
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
            .GreaterThanOrEqualTo(1)
            .WithMessage("Should be greater than or equal to 1");


        RuleFor(model => model.GenesModel)
            .NotEmpty()
            .When(model => model.CellsNumber != null)
            .WithMessage("Should not be empty if 'cells_number' is set");

        RuleFor(model => model.GenesModel)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");


        RuleForEach(model => model.Resources)
            .SetValidator(_resourceModelValidator);
    }
}
