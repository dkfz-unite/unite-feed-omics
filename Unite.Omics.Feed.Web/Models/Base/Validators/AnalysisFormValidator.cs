using FluentValidation;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base.Extensions;

namespace Unite.Omics.Feed.Web.Models.Base.Validators;

public class AnalysisFormValidator<T> : AbstractValidator<AnalysisForm<T>>
    where T : class, new()
{
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


        RuleFor(model => model.Genome)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model)
            .Must(HaveValidGenome)
            .When(model => !string.IsNullOrWhiteSpace(model.Genome))
            .WithMessage($"Only '{GenomeOptions.Build}' reference genome is supported for DNA and bulk RNA samples");


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
    

    private static bool HaveValidGenome(AnalysisForm<T> model)
    {
        var analyses = new AnalysisType?[] { AnalysisType.WGS, AnalysisType.WES, AnalysisType.RNASeq };

        if (analyses.Contains(model.AnalysisType))
            return string.Equals(model.Genome, GenomeOptions.Build, StringComparison.InvariantCultureIgnoreCase);
        else
            return true;
    }
}
