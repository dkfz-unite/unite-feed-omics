using FluentValidation;

namespace Unite.Mutations.Feed.Web.Models.Mutations.Validators
{
    public class AnalysisModelValidator : AbstractValidator<AnalysisModel>
    {
        private readonly IValidator<FileModel> _fileModelValidator;

        public AnalysisModelValidator()
        {
            _fileModelValidator = new FileModelValidator();


            RuleFor(model => model.Type)
                .NotEmpty()
                .WithMessage("Should not be empty");


            RuleFor(model => model.File)
                .SetValidator(_fileModelValidator)
                .When(model => model.File != null);
        }
    }
}
