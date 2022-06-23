using FluentValidation;
using Unite.Genome.Feed.Web.Services.Mutations;

namespace Unite.Genome.Feed.Web.Services.Mutations.Validators;

public class AnalysisModelValidator : AbstractValidator<AnalysisModel>
{
    public AnalysisModelValidator()
    {
        RuleFor(model => model.Type)
            .NotEmpty()
            .WithMessage("Should not be empty");
    }
}
