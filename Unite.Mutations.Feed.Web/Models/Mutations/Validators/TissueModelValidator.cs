using FluentValidation;
using Unite.Data.Entities.Specimens.Tissues.Enums;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public class TissueModelValidator : SpecimenModelValidator<TissueModel>
    {
        public TissueModelValidator() : base()
        {
            RuleFor(model => model.TumourType)
                .Empty()
                .When(model => model.Type != TissueType.Tumour)
                .WithMessage("Tumour type can be set only for tumour tissues");

            RuleFor(model => model.Source)
                .MaximumLength(100)
                .WithMessage("Maximum length is 100");

        }
    }
}
