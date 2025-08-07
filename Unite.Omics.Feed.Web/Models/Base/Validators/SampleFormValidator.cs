using FluentValidation;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Omics.Feed.Web.Models.Base.Binders;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class SampleFormValidator : AbstractValidator<SampleForm>
{
    private readonly StringComparison _comparison = StringComparison.InvariantCultureIgnoreCase;

    public SampleFormValidator()
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

        RuleFor(model => model.SpecimenType)
            .Must(value => EnumBinder.Bind<SpecimenType>(value) != null)
            .WithMessage($"Invalid value");


        RuleFor(model => model.AnalysisType)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.AnalysisType)
            .Must(value => EnumBinder.Bind<AnalysisType>(value) != null)
            .WithMessage($"Invalid value");


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


        RuleFor(model => model.Genome)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Genome)
            .Must(genome => genome.Equals("GRCh37", _comparison) || genome.Equals("GRCh38", _comparison));


        RuleFor(model => model.ResourcesFile)
            .NotEmpty()
            .WithMessage("Should not be empty");
    }
}
