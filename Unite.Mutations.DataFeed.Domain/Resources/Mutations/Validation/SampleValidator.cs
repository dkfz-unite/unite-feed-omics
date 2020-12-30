using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Validation
{
    public class SampleValidator : AbstractValidator<Sample>
    {
        public SampleValidator()
        {
            RuleFor(sample => sample.Name)
                .NotEmpty().When(sample => !sample.Type.HasValue).WithMessage("Should not be empty if 'Type' is not set")
                .MaximumLength(500).WithMessage("Maximum length is 500");

            RuleFor(sample => sample.Link)
                .MaximumLength(500).WithMessage("Maximum length is 500");

            RuleFor(sample => sample.Type)
                .NotEmpty().When(sample => string.IsNullOrWhiteSpace(sample.Name)).WithMessage("Should not be empty if 'Name' is not set");

            RuleFor(sample => sample.Subtype)
               .Empty().When(sample => sample.Type == Data.Entities.Samples.Enums.SampleType.Control).WithMessage("Sample 'Subtype' can be set only for samples of type 'Tumor'");
        }
    }
}
