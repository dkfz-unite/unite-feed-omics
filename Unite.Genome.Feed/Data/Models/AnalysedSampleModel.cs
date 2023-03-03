namespace Unite.Genome.Feed.Data.Models;

public class AnalysedSampleModel
{
    public SampleModel AnalysedSample;
    public SampleModel MatchedSample;

    public IEnumerable<Variants.SSM.VariantModel> Mutations;
    public IEnumerable<Variants.CNV.VariantModel> CopyNumberVariants;
    public IEnumerable<Variants.SV.VariantModel> StructuralVariants;
    public IEnumerable<Transcriptomics.GeneExpressionModel> GeneExpressions;
}
