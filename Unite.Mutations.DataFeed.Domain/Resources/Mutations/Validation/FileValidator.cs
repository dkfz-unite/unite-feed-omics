using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Validation
{
    public class FileValidator : AbstractValidator<File>
    {
        public FileValidator()
        {
            RuleFor(file => file.Name)
                .NotEmpty().WithMessage("Should not be empty");
        }
    }
}
