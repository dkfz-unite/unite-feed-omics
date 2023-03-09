using FluentValidation;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Validators;

public class AnalysedSampleModelValidator<TModel, TValidator> : Variants.Validators.AnalysedSampleModelValidator<TModel, TValidator>
    where TModel : class, new()
    where TValidator : IValidator<TModel>, new()
{
    public AnalysedSampleModelValidator() : base()
    {

    }
}
