using FluentValidation;

namespace Unite.Mutations.Feed.Web.Resources.Mutations.Validators
{
    public class FileResourceValidator : AbstractValidator<FileResource>
    {
        public FileResourceValidator()
        {
            RuleFor(resource => resource.Name)
                .NotEmpty().WithMessage("Should not be empty");
        }
    }
}
