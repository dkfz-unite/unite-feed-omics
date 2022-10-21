using FluentValidation;
using Unite.Data.Entities.Genome.Analysis.Enums;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public abstract class AnalysisModelValidator<T> : AbstractValidator<T> where T : AnalysisModel
{
    public abstract AnalysisType[] AllowedTypes { get; }


    public AnalysisModelValidator()
    {
        RuleFor(model => model.Id)
            .MaximumLength(255)
            .WithMessage("Maximum length is 255");

        RuleFor(model => model.Type)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Type)
            .Must(value => AllowedTypes.Contains(value.Value))
            .WithMessage(GetAllowedTypesErrorMessage());
    }


    protected virtual string GetAllowedTypesErrorMessage()
    {
        var types = AllowedTypes.Select(type => $"'{type}'");

        return $"Should be one of the following types: {string.Join(", ", types)}";
    }
}
