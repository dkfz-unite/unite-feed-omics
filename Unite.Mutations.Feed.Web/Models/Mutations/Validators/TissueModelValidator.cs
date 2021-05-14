using FluentValidation;
using Unite.Data.Entities.Specimens.Tissues.Enums;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public class TissueModelValidator : AbstractValidator<TissueModel>
    {
        public TissueModelValidator()
        {
            RuleFor(model => model.Id)
                .MaximumLength(255)
                .WithMessage("Maximum length is 255");

            RuleFor(model => model.Id)
                .NotEmpty()
                .When(model => string.IsNullOrWhiteSpace(model.DonorId))
                .WithMessage("Tissue external identifier should be set if donor external identifier is empty");

            RuleFor(model => model.DonorId)
                .MaximumLength(255)
                .WithMessage("Maximum length is 255");

            RuleFor(model => model.Type)
                .NotEmpty()
                .When(model => string.IsNullOrWhiteSpace(model.Id))
                .WithMessage("Tissue type should be set if tissue external identifier is empty");

            RuleFor(model => model.TumourType)
                .Empty()
                .When(model => model.Type == TissueType.Control)
                .WithMessage("Tumour type can be set only for tumour tissues");

        }
    }
}
