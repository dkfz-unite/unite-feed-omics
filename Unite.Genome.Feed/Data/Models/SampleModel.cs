namespace Unite.Genome.Feed.Data.Models;

public class SampleModel
{
    public string Genome;
    public double? Purity;
    public double? Ploidy;
    public int? Cells;

    public SpecimenModel Specimen;
    public AnalysisModel Analysis;
    public SampleModel MatchedSample;
    
    public IEnumerable<Dna.Sm.VariantModel> Sms;
    public IEnumerable<Dna.Cnv.VariantModel> Cnvs;
    public IEnumerable<Dna.Sv.VariantModel> Svs;
    public IEnumerable<Rna.GeneExpressionModel> Exps;
    public IEnumerable<ResourceModel> Resources;
}
