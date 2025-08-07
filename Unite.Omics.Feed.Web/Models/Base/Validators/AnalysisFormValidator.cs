using FluentValidation;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Omics.Feed.Web.Models.Base.Binders;
using Unite.Omics.Feed.Web.Models.Base.Extensions;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class AnalysisFormValidator<T> : AbstractValidator<AnalysisForm<T>>
    where T : class, new()
{
    private readonly StringComparison _comparison = StringComparison.InvariantCultureIgnoreCase;


    public AnalysisFormValidator()
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


        RuleFor(model => model.MatchedSpecimenId)
            .NotEmpty()
            .When(model => model.MatchedSpecimenType != null)
            .WithMessage("Should not be empty");

        RuleFor(model => model.MatchedSpecimenId)
            .MaximumLength(255)
            .When(model => model.MatchedSpecimenType != null)
            .WithMessage("Maximum length is 255");


        RuleFor(model => model.MatchedSpecimenType)
            .NotEmpty()
            .When(model => model.MatchedSpecimenId != null)
            .WithMessage("Should not be empty");

        RuleFor(model => model.MatchedSpecimenType)
            .Must(value => EnumBinder.Bind<SpecimenType>(value) != null)
            .When(model => model.MatchedSpecimenId != null)
            .WithMessage($"Invalid value");


        RuleFor(model => model.AnalysisType)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.AnalysisType)
            .Must(value => EnumBinder.Bind<AnalysisType>(value) != null)
            .WithMessage($"Invalid value");


        RuleFor(model => model.Genome)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Genome)
            .Must(genome => genome.Equals("GRCh37", _comparison) || genome.Equals("GRCh38", _comparison));


        RuleFor(model => model.Purity)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1)
            .WithMessage("Should be in range [0, 1]");


        RuleFor(model => model.Ploidy)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Should be greater than or equal to 1");


        RuleFor(model => model.Cells)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Should be greater than or equal to 1");


        RuleFor(model => model.ResourcesFile)
            .NotEmpty()
            .When(model => model.EntriesFile.IsEmpty())
            .WithMessage("Should not be empty if `entries` are not set");


        RuleFor(model => model.EntriesFile)
            .NotEmpty()
            .When(model => model.ResourcesFile.IsEmpty())
            .WithMessage("Should not be empty if `resources` are not set");
    }
}
