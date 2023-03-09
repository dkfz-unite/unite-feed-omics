using FluentValidation;
using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Models.Variants.Validators;

/// <summary>
/// Variant type specific sequencing data model validator
/// </summary>
/// <typeparam name="TModel">Variant model type</typeparam>
/// <typeparam name="TModelValidator">Variant model validator type</typeparam>
public class SequencingDataModelValidator<TModel, TModelValidator> : AbstractValidator<SequencingDataModel<TModel>>
    where TModel : class, IDistinctable, new()
    where TModelValidator : IValidator<TModel>, new()
{
    private readonly IValidator<AnalysisModel> _analysisModelValidator;
    private readonly IValidator<AnalysedSampleModel<TModel>> _analysedSampleModelValidator;

    public SequencingDataModelValidator()
    {
        _analysisModelValidator = new AnalysisModelValidator();
        _analysedSampleModelValidator = new AnalysedSampleModelValidator<TModel, TModelValidator>();


        RuleFor(model => model.Analysis)
            .SetValidator(_analysisModelValidator)
            .When(model => model.Analysis != null);


        RuleFor(model => model.Samples)
            .NotEmpty()
            .WithMessage("Should not be empty");

        RuleForEach(model => model.Samples)
            .SetValidator(_analysedSampleModelValidator);


        RuleFor(model => model)
            .Must(EachSampleIdIsUnique)
            .When(model => model.Samples != null)
            .WithMessage("Each sample id should be unique");

        RuleFor(model => model)
            .Must(EachMatchedSampleIdMatchesSingleAnalysedSampleId)
            .When(model => model.Samples != null)
            .WithMessage("Each matched sample id should match single analysed sample id");
    }

    private static bool EachSampleIdIsUnique(SequencingDataModel<TModel> model)
    {
        var ids = model.Samples.Select(sample => sample.Id);

        return ids.Count() == ids.Distinct().Count();
    }

    private static bool EachMatchedSampleIdMatchesSingleAnalysedSampleId(SequencingDataModel<TModel> model)
    {
        foreach (var analysedSample in model.Samples.Where(sample => !string.IsNullOrWhiteSpace(sample.MatchedSampleId)))
        {
            var matchedSamplesNumber = model.Samples.Count(sample => string.Equals(sample.Id, analysedSample.MatchedSampleId));

            if (matchedSamplesNumber != 1)
            {
                return false;
            }
        }

        return true;
    }
}
