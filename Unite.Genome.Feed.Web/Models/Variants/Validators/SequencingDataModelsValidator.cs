using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Variants.Validators;

/// <summary>
/// Variant type specific sequencing data models validator
/// </summary>
/// <typeparam name="TModel">Variant model type</typeparam>
/// <typeparam name="TModelValidator">Variant model validator type</typeparam>
public class SequencingDataModelsValidator<TModel, TModelValidator> : AbstractValidator<IEnumerable<SequencingDataModel<TModel>>>
    where TModel : class, new()
    where TModelValidator : IValidator<TModel>, new()
{
    private readonly IValidator<SequencingDataModel<TModel>> _sequencingDataModelValidator;

    public SequencingDataModelsValidator()
    {
        _sequencingDataModelValidator = new SequencingDataModelValidator<TModel, TModelValidator>();

        RuleForEach(model => model)
            .SetValidator(_sequencingDataModelValidator);
    }
}
