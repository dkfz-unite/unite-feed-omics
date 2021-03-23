using FluentValidation;

namespace Unite.Mutations.Feed.Web.Resources.Mutations.Validators
{
    public class AnalysisResourceValidator : AbstractValidator<AnalysisResource>
    {
        private readonly IValidator<FileResource> _fileResourceValidator;

        public AnalysisResourceValidator()
        {
            _fileResourceValidator = new FileResourceValidator();

            RuleFor(resource => resource.Name)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(500).WithMessage("Maximum length is 500");

            RuleFor(resource => resource.File)
                .SetValidator(_fileResourceValidator).When(analysis => analysis.File != null);
        }
    }
}
