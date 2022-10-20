using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Transcriptome.Validators;

public class ExpressionModelValidator : AbstractValidator<ExpressionModel>
{
    public ExpressionModelValidator()
    {
        RuleFor(model => model.GeneSymbol)
            .NotEmpty()
            .WithMessage("Should not be empty");


        RuleFor(model => model.Reads)
            .NotEmpty()
            .When(model => model.FPKM == null)
            .WithMessage("Either 'Reads' count or 'FPKM' normalized reads count should be set");

        RuleFor(model => model.Reads)
            .Must(value => value >= 0)
            .WithMessage("Should be higher than or equal to 0");


        RuleFor(model => model.FPKM)
            .NotEmpty()
            .When(model => model.Reads == null)
            .WithMessage("Either 'Reads' count or 'FPKM' normalized reads count should be set");

        RuleFor(model => model.FPKM)
            .Must(value => value >= 0)
            .WithMessage("Should be higher than or equal to 0");
    }
}
