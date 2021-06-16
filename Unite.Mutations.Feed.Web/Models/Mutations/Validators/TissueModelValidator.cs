using FluentValidation;
using Unite.Data.Entities.Specimens.Tissues.Enums;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public class TissueModelValidator : SpecimenModelValidator<TissueModel>
    {
        public TissueModelValidator() : base()
        {
            RuleFor(model => model.TumorType)
                .Empty()
                .When(model => model.Type != TissueType.Tumor)
                .WithMessage("Tumor type can be set only for tumor tissues");

            RuleFor(model => model.Source)
                .MaximumLength(100)
                .WithMessage("Maximum length is 100");

        }
    }
}
