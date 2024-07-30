using FluentValidation;
using Unite.Essentials.Extensions;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class AnalysisModelValidator<T, TValidator> : AbstractValidator<AnalysisModel<T>>
    where T : class, new()
    where TValidator : IValidator<T>, new()
{
    private readonly IValidator<SampleModel> _sampleModelValidator;
    private readonly IValidator<T> _entryModelValidator;

    public AnalysisModelValidator()
    {
        _sampleModelValidator = new SampleModelValidator();
        _entryModelValidator = new TValidator();

        RuleFor(model => model.TargetSample)
            .NotEmpty().WithMessage("Should not be empty")
            .SetValidator(_sampleModelValidator);

        RuleFor(model => model.MatchedSample)
            .SetValidator(_sampleModelValidator)
            .When(model => model.MatchedSample != null);

        RuleForEach(model => model.Entries)
            .SetValidator(_entryModelValidator)
            .When(model => model.Entries != null);

        RuleFor(model => model)
            .Must(HaveData)
            .WithMessage("Either 'entries' or data 'resource' should be set");
    }


    private bool HaveData(AnalysisModel<T> model)
    {
        var hasEntries = model.Entries.IsNotEmpty();
        
        var hasResources = model.TargetSample.Resources?.FirstOrDefault(resource => string.Equals(resource.Type, "data", StringComparison.InvariantCultureIgnoreCase)) != null;

        return hasEntries || hasResources;
    }
}
