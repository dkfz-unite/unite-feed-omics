using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Transcriptomics.Validators;

public class BulkExpressionModelValidator : AbstractValidator<BulkExpressionModel>
{
    public BulkExpressionModelValidator()
    {
        RuleFor(model => model.GetDataType())
            .Must(value => value != 0)
            .WithMessage("Either 'GeneId', 'GeneSymbol', 'TranscriptId' or 'TranscriptSymbol' has to be specified");

        RuleFor(model => model.GeneId)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.GeneSymbol)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.TranscriptId)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");
        
        RuleFor(model => model.TranscriptSymbol)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Source)
            .NotEmpty().WithMessage("Should not be empty")
            .MaximumLength(100).WithMessage("Maximum length is 100");

        RuleFor(model => model.ExonicLength)
            .Must(value => value > 0)
            .When(model => model.ExonicLength != null)
            .WithMessage("Should be greater than 0");

        RuleFor(model => model.Reads)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Reads)
            .Must(value => value >= 0)
            .WithMessage("Should be greater than or equal to 0");
    }
}
