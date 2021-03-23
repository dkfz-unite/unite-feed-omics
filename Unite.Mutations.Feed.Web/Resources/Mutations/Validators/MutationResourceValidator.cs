using FluentValidation;

namespace Unite.Mutations.Feed.Web.Resources.Mutations.Validators
{
    public class MutationResourceValidator : AbstractValidator<MutationResource>
    {
        public MutationResourceValidator()
        {
            RuleFor(resource => resource.Chromosome)
                .NotEmpty().WithMessage("Should not be empty");

            RuleFor(resource => resource.SequenceType)
                .NotEmpty().WithMessage("Should not be empty");

            RuleFor(resource => resource.Position)
                .NotEmpty().WithMessage("Should not be empty")
                .Must(IsNumberOrRange).WithMessage("Should be number '1234567890' or range '1234567890-1234567890'");

            RuleFor(resource => resource.Ref)
                .NotEmpty().When(resource => string.IsNullOrWhiteSpace(resource.Alt)).WithMessage("At least one of 'Ref' or 'Alt' fields has to be set")
                .MaximumLength(200).WithMessage("Maximum length is 200");

            RuleFor(resource => resource.Alt)
                .NotEmpty().When(resource => string.IsNullOrWhiteSpace(resource.Ref)).WithMessage("At least one of 'Ref' or 'Alt' fields has to be set")
                .MaximumLength(200).WithMessage("Maximum length is 200");
        }

        private bool IsNumberOrRange(string position)
        {
            var normalized = position.Trim();

            if (normalized.Contains('-'))
            {
                var blocks = normalized.Split('-');

                if(blocks.Length != 2)
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
}
