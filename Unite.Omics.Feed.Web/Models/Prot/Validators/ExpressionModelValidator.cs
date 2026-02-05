using FluentValidation;

namespace Unite.Omics.Feed.Web.Models.Prot.Validators;

public class ExpressionModelValidator : AbstractValidator<ExpressionModel>
{
    public ExpressionModelValidator()
    {
        RuleFor(model => model.GetKeyType())
            .Must(value => value != 0)
            .WithMessage("Either 'gene_id', 'gene_symbol', 'transcript_id', 'transcript_symbol', 'protein_id', 'protein_accession' or 'protein_symbol' has to be specified");

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

        RuleFor(model => model.ProteinId)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.ProteinAccession)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.ProteinSymbol)
            .MaximumLength(100)
            .WithMessage("Maximum length is 100");

        RuleFor(model => model.Intensity)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Intensity)
            .Must(value => value >= 0)
            .WithMessage("Should be greater than or equal to 0");
    }
}
