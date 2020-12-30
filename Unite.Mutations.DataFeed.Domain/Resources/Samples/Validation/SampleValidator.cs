using System.Collections.Generic;
using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Resources.Samples.Validation
{
    public class SampleValidator : AbstractValidator<Sample>
    {
        private readonly IValidator<Mutation> _mutationValidator;

        public SampleValidator()
        {
            _mutationValidator = new MutationValidator();

            RuleFor(sample => sample.Pid)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(100).WithMessage("Maximum length is 100");

            RuleFor(sample => sample.Name)
                .MaximumLength(500).WithMessage("Maximum length is 500");

            RuleFor(sample => sample.Link)
                .MaximumLength(500).WithMessage("Maximum length is 500");

            RuleFor(sample => sample.Subtype)
                .Empty().When(sample => sample.Type == Data.Entities.Samples.Enums.SampleType.Control).WithMessage("Sample 'Subtype' can be set only for samples of type 'Tumor'");

            RuleForEach(sample => sample.Mutations)
                .SetValidator(_mutationValidator).When(sample => sample.Mutations != null);
        }
    }

    public class SamplesValidator : AbstractValidator<IEnumerable<Sample>>
    {
        private readonly IValidator<Sample> _sampleValidator;

        public SamplesValidator()
        {
            _sampleValidator = new SampleValidator();

            RuleForEach(samples => samples)
                .SetValidator(_sampleValidator).When(samples => samples != null);
        }
    }
}
