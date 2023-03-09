using FluentValidation;
using Unite.Genome.Feed.Web.Models.Base.Validators;

namespace Unite.Genome.Feed.Web.Models.Variants.Validators;

/// <summary>
/// Variant type specific analysed sample model validator
/// </summary>
/// <typeparam name="TModel">Variant model type</typeparam>
/// <typeparam name="TModelValidator">Variant model validator validator</typeparam>
public class AnalysedSampleModelValidator<TModel, TModelValidator> : SampleModelValidator<AnalysedSampleModel<TModel>>
    where TModel : class, new()
    where TModelValidator : IValidator<TModel>, new()
{
    private readonly IValidator<TModel> _variantModelValidator;

    public AnalysedSampleModelValidator() : base()
    {
        _variantModelValidator = new TModelValidator();

        //RuleFor(model => model.Ploidy)
        //    .NotEmpty().WithMessage("Should not be empty")
        //    .When(IsCnvSequencingSample);

        RuleForEach(model => model.Variants)
            .SetValidator(_variantModelValidator);
    }

    private static bool IsCnvSequencingSample(AnalysedSampleModel<TModel> model)
    {
        return typeof(TModel) == typeof(CNV.VariantModel)
            || typeof(TModel) == typeof(CNV.VariantAceSeqModel);
    }
}
