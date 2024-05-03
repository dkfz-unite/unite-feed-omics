using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class ResourceModelValidator : AbstractValidator<ResourceModel>
{
    private static readonly string[] _allowedTypes =
    {
        "tsv",
        "csv[(delimeter)]", // csv or csv(;), "," is default delimeter
        "mtx", // Matrix Market format
    };

    public ResourceModelValidator()
    {
        RuleFor(model => model.Type)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleFor(model => model.Type)
            .Must(type => _allowedTypes.Contains(type))
            .WithMessage("Type is not allowed");

        RuleFor(model => model.Path)
            .NotEmpty()
            .When(model => string.IsNullOrEmpty(model.Url))
            .WithMessage("Should not be empty if 'url' is not set");

        RuleFor(model => model.Url)
            .NotEmpty()
            .When(model => string.IsNullOrEmpty(model.Path))
            .WithMessage("Should not be empty if 'path' is not set");
    }
}
