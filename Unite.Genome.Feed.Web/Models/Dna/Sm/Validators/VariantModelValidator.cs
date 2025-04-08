using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Dna.Sm.Validators;

public class VariantModelValidator : AbstractValidator<VariantModel>
{
    public VariantModelValidator()
    {
        RuleFor(model => model.Chromosome)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Position)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Position)
            .Must(IsNumberOrRange)
            .WithMessage("Should be number '1234567890' or range '1234567890-1234567890'");

        RuleFor(model => model.Ref)
            .NotEmpty()
            .When(model => string.IsNullOrWhiteSpace(model.Alt))
            .WithMessage("At least one of 'Ref' or 'Alt' fields has to be set");

        RuleFor(model => model.Ref)
            .MaximumLength(200)
            .WithMessage("Maximum length is 200");

        RuleFor(model => model.Alt)
            .NotEmpty().When(model => string.IsNullOrWhiteSpace(model.Ref))
            .WithMessage("At least one of 'Ref' or 'Alt' fields has to be set");

        RuleFor(model => model.Alt)
            .MaximumLength(200)
            .WithMessage("Maximum length is 200");
    }


    private bool IsNumberOrRange(string position)
    {
        var normalized = position.Trim();

        if (normalized.Contains('-'))
        {
            var blocks = normalized.Split('-');

            if (blocks.Length != 2)
            {
                return false;
            }
            else
            {
                var startIsNumber = int.TryParse(blocks[0], out _);
                var endIsNumber = int.TryParse(blocks[1], out _);

                return startIsNumber && endIsNumber;
            }
        }
        else
        {
            var isNumber = int.TryParse(normalized, out _);

            return isNumber;
        }
    }
}
