using Unite.Data.Entities.Genome.Analysis.Enums;

namespace Unite.Genome.Feed.Web.Models.Transcriptome.Validators;

public class AnalysisModelValidator : Base.Validators.AnalysisModelValidator<AnalysisModel>
{
    public override AnalysisType[] AllowedTypes => new AnalysisType[]
    {
        AnalysisType.RNASeq,
        AnalysisType.Other
    };


    public AnalysisModelValidator() : base()
    {
    }
}
