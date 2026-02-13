using FluentValidation;

namespace Unite.Omics.Feed.Web.Models.Rna.Validators;

public class ExpressionModelValidator : AbstractValidator<ExpressionModel>
{
    public ExpressionModelValidator()
    {
        RuleFor(model => model.GetKeyType())
            .Must(value => value != 0)
            .WithMessage("Either 'gene_id', 'gene_symbol', 'transcript_id' or 'transcript_symbol' has to be specified");

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
