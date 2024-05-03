namespace Unite.Genome.Feed.Data.Models;

public class AnalysedSampleModel : SampleModel
{
    public AnalysisModel Analysis;
    public SpecimenModel TargetSample;
    public SpecimenModel MatchedSample;

    public IEnumerable<ResourceModel> Resources;
    public IEnumerable<Variants.SSM.VariantModel> Ssms;
    public IEnumerable<Variants.CNV.VariantModel> Cnvs;
    public IEnumerable<Variants.SV.VariantModel> Svs;
    public IEnumerable<Transcriptomics.BulkExpressionModel> BulkExpressions;
}
