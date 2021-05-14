using FluentValidation;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public class AnalysedSampleModelValidator : AbstractValidator<AnalysedSampleModel>
    {
        private readonly IValidator<TissueModel> _tissueModelValidator;
        private readonly IValidator<MutationModel> _mutationModelValidator;

        public AnalysedSampleModelValidator()
        {
            _tissueModelValidator = new TissueModelValidator();
            _mutationModelValidator = new MutationModelValidator();


            RuleFor(model => model.Id)
                .MaximumLength(255)
                .WithMessage("Maximum length is 255");

            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Should not be empty");


            RuleFor(model => model.Tissue)
                .SetValidator(_tissueModelValidator)
                .When(model => model.Tissue != null);

            RuleForEach(model => model.Mutations)
                .SetValidator(_mutationModelValidator)
                .When(model => model.Mutations != null);
        }
    }
}
