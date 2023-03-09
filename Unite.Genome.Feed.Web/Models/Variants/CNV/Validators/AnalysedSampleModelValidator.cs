using FluentValidation;
using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Validators;

public class AnalysedSampleModelValidator<TModel, TValidator> : Variants.Validators.AnalysedSampleModelValidator<TModel, TValidator>
    where TModel : class, IDistinctable, new()
    where TValidator : IValidator<TModel>, new()
{
    public AnalysedSampleModelValidator() : base()
    {

    }
}
