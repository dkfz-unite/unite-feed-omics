using FluentValidation;
using Unite.Data.Entities.Genome.Analysis.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Genome.Feed.Web.Models.Base.Validators;

public class AnalysisModelValidator<T, TValidator> : AbstractValidator<AnalysisModel<T>>
    where T : class, new()
    where TValidator : IValidator<T>, new()
{
    private readonly IValidator<SampleModel> _sampleModelValidator;
    private readonly IValidator<ResourceModel> _resourceModelValidator;
    private readonly IValidator<T> _entryModelValidator;


    public AnalysisModelValidator()
    {
        _sampleModelValidator = new SampleModelValidator();
        _resourceModelValidator = new ResourceModelValidator();
        _entryModelValidator = new TValidator();

        RuleFor(model => model.TargetSample)
            .NotEmpty().WithMessage("Should not be empty")
            .SetValidator(_sampleModelValidator);

        RuleFor(model => model.MatchedSample)
            .SetValidator(_sampleModelValidator)
            .When(model => model.MatchedSample != null);


        RuleFor(model => model.Resources)
            .NotEmpty().WithMessage("Should not be empty if `entries` are not set")
            .When(model => model.Entries.IsEmpty());

        RuleForEach(model => model.Resources)
            .SetValidator(_resourceModelValidator)
            .When(model => model.Resources.IsNotEmpty());


        RuleFor(model => model.Entries)
            .NotEmpty().WithMessage("Should not be empty if `resources` are not set")
            .When(model => model.Resources.IsEmpty());

        RuleForEach(model => model.Entries)
            .SetValidator(_entryModelValidator)
            .When(model => model.Entries.IsNotEmpty());


        RuleFor(model => model)
            .Must(model => HaveCorrectGenome(model.TargetSample))
            .WithMessage($"Only '{SampleModel.DefaultGenome}' reference genome is supported for DNA samples")
            .When(model => model.TargetSample != null);

        RuleFor(model => model)
            .Must(model => HaveCorrectGenome(model.MatchedSample))
            .WithMessage($"Only '{SampleModel.DefaultGenome}' reference genome is supported for DNA samples")
            .When(model => model.MatchedSample != null);
    }


    private static bool HaveCorrectGenome(SampleModel model)
    {
        if (model.AnalysisType == AnalysisType.WGS || model.AnalysisType == AnalysisType.WES)
            return model.Genome == SampleModel.DefaultGenome;
        else
            return true;
    }
}
