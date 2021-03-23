using FluentValidation;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Web.Resources.Mutations.Validators
{
    public class SampleResourceValidator : AbstractValidator<SampleResource>
    {
        private readonly IValidator<MutationResource> _mutationResourceValidator;

        public SampleResourceValidator()
        {
            _mutationResourceValidator = new MutationResourceValidator();

            RuleFor(resource => resource.Name)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(500).WithMessage("Maximum length is 500");

            RuleFor(resource => resource.Subtype)
                .Empty().When(resource => resource.Type == SampleType.Control).WithMessage("Sample 'Subtype' can be set only for samples of type 'Tumor'");

            RuleForEach(resource => resource.Mutations)
                .SetValidator(_mutationResourceValidator).When(sample => sample.Mutations != null);
        }
    }
}
