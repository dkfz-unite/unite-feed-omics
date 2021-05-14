using FluentValidation;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public class FileModelValidator : AbstractValidator<FileModel>
    {
        public FileModelValidator()
        {
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Should not be empty");
        }
    }
}
