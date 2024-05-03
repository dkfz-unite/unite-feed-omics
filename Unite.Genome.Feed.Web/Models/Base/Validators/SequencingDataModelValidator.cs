using FluentValidation;
using Unite.Essentials.Extensions;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class SequencingDataModelValidator<T, TValidator> : AbstractValidator<SequencingDataModel<T>>
    where T : class, new()
    where TValidator : IValidator<T>, new()
{
    private readonly IValidator<AnalysisModel> _analysisModelValidator;
    private readonly IValidator<SampleModel> _sampleModelValidator;
    private readonly IValidator<ResourceModel> _resourceModelValidator;
    private readonly IValidator<T> _entryModelValidator;

    public SequencingDataModelValidator()
    {
        _analysisModelValidator = new AnalysisModelValidator();
        _sampleModelValidator = new SampleModelValidator();
        _resourceModelValidator = new ResourceModelValidator();
        _entryModelValidator = new TValidator();

        RuleFor(model => model.Analysis)
            .NotEmpty().WithMessage("Should not be empty")
            .SetValidator(_analysisModelValidator);

        RuleFor(model => model.TargetSample)
            .NotEmpty().WithMessage("Should not be empty")
            .SetValidator(_sampleModelValidator);

        RuleFor(model => model.MatchedSample)
            .SetValidator(_sampleModelValidator)
            .When(model => model.MatchedSample != null);

        RuleFor(model => model.Resources)
            .NotEmpty()
            .When(model => model.Entries.IsEmpty())
            .WithMessage("Should not be empty if 'entries' is empty");

        RuleForEach(model => model.Resources)
            .SetValidator(_resourceModelValidator)
            .When(model => model.Resources != null);

        RuleFor(model => model.Entries)
            .NotEmpty()
            .When(model => model.Resources.IsEmpty())
            .WithMessage("Should not be empty if 'resources' is empty");

        RuleForEach(model => model.Entries)
            .SetValidator(_entryModelValidator)
            .When(model => model.Entries != null);
    }
}
