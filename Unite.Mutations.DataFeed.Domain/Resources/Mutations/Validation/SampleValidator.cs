using FluentValidation;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Validation
{
    public class SampleValidator : AbstractValidator<Sample>
    {
        private readonly IValidator<Mutation> _mutationValidator;

        public SampleValidator()
        {
            _mutationValidator = new MutationValidator();

            RuleFor(sample => sample.Name)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(500).WithMessage("Maximum length is 500");

            RuleFor(sample => sample.Subtype)
                .Empty().When(sample => sample.Type == SampleType.Control).WithMessage("Sample 'Subtype' can be set only for samples of type 'Tumor'");

            RuleForEach(sample => sample.Mutations)
                .SetValidator(_mutationValidator).When(sample => sample.Mutations != null);
        }
    }
}
