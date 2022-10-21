using Unite.Genome.Feed.Data.Models.Variants;

namespace Unite.Genome.Feed.Data.Models;

public class AnalysedSampleModel
{
    public SampleModel AnalysedSample { get; set; }
    public SampleModel MatchedSample { get; set; }

    public IEnumerable<Variants.SSM.VariantModel> Mutations { get; set; }
    public IEnumerable<Variants.CNV.VariantModel> CopyNumberVariants { get; set; }
    public IEnumerable<Variants.SV.VariantModel> StructuralVariants { get; set; }
}
