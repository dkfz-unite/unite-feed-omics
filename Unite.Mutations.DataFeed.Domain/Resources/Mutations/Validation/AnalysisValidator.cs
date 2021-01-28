using FluentValidation;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Validation
{
    public class AnalysisValidator : AbstractValidator<Analysis>
    {
        private readonly IValidator<File> _fileValidator;

        public AnalysisValidator()
        {
            _fileValidator = new FileValidator();

            RuleFor(analysis => analysis.Name)
                .NotEmpty().WithMessage("Should not be empty")
                .MaximumLength(500).WithMessage("Maximum length is 500");

            RuleFor(analysis => analysis.File)
                .SetValidator(_fileValidator).When(analysis => analysis.File != null);
        }
    }
}
