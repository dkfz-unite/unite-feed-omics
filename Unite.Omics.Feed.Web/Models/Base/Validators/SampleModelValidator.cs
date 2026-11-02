using FluentValidation;
using Unite.Data.Entities.Omics.Analysis.Enums;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class SampleModelValidator : AbstractValidator<SampleModel>
{
    private readonly StringComparison _comparison = StringComparison.InvariantCultureIgnoreCase;


    public SampleModelValidator(AnalysisType[] assayTypes)
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
            .NotNull()
            .WithMessage("Should not be empty");

        RuleFor(model => model.SpecimenType)
            .IsInEnum()
            .WithMessage($"Invalid value");


        RuleFor(model => model.AssayType)
            .NotNull()
            .WithMessage("Should not be empty");

        RuleFor(model => model.AssayType)
            .Must(value => assayTypes.Contains(value.Value))
            .When(model => model.AssayType != null)
            .WithMessage($"Invalid value");


        RuleFor(model => model.AssayDate)
            .Empty()
            .When(model => model.AssayDay != null)
            .WithMessage("Either exact 'analysis_date' or relative 'analysis_day' can be set, not both");


        RuleFor(model => model.AssayDay)
            .Empty()
            .When(model => model.AssayDate != null)
            .WithMessage("Either exact 'analysis_date' or relative 'analysis_day' can be set, not both");

        RuleFor(model => model.AssayDay)
            .GreaterThanOrEqualTo(1)
            .When(model => model.AssayDay != null)
            .WithMessage("Should be greater than or equal to 1");


        RuleFor(model => model.Genome)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Genome)
            .Must(genome => string.Equals(genome, "GRCh37", _comparison) || string.Equals(genome, "GRCh38", _comparison))
            .WithMessage("Should be either 'GRCh37' or 'GRCh38'");


        RuleFor(model => model.Batch)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");


        RuleFor(model => model.Purity).InclusiveBetween(0, 1);
        RuleFor(model => model.Ploidy).GreaterThanOrEqualTo(1);
        RuleFor(model => model.Cells).GreaterThanOrEqualTo(1);


        RuleForEach(model => model.Resources)
            .NotNull().WithMessage("Should not be empty")
            .SetValidator(new ResourceModelValidator());
    }
}
