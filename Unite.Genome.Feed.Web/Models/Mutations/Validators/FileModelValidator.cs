using FluentValidation;

namespace Unite.Genome.Feed.Web.Services.Mutations.Validators
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
