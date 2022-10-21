using Unite.Data.Entities.Genome.Analysis.Enums;
using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Models.Variants.Validators;

public class AnalysisModelValidator : Models.Base.Validators.AnalysisModelValidator<AnalysisModel>
{
    public override AnalysisType[] AllowedTypes => new AnalysisType[]
    {
        AnalysisType.WGS,
        AnalysisType.WES,
        AnalysisType.Other
    };


    public AnalysisModelValidator() : base()
    {
    }
}
