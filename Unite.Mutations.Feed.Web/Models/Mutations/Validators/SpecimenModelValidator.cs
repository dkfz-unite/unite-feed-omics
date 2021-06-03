using FluentValidation;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public abstract class SpecimenModelValidator<TModel> : AbstractValidator<TModel>
        where TModel : SpecimenModel
    {
        public SpecimenModelValidator()
        {
            RuleFor(model => model.Id)
                .NotEmpty()
                .WithMessage("Should not be empty");

            RuleFor(model => model.Id)
                .MaximumLength(255)
                .WithMessage("Maximum length is 255");


            RuleFor(model => model.DonorId)
                .NotEmpty()
                .WithMessage("Should not be empty");

            RuleFor(model => model.DonorId)
                .MaximumLength(255)
                .WithMessage("Maximum length is 255");
        }
    }
}
