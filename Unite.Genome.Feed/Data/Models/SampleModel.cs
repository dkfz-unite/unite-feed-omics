namespace Unite.Genome.Feed.Data.Models;

public class SampleModel
{
    public double? Purity { get; set; }
    public double? Ploidy { get; set; }
    public int? CellsNumber { get; set; }
    public string GenesModel { get; set; }

    public SpecimenModel Specimen;
    public AnalysisModel Analysis;
    public SampleModel MatchedSample;
    
    public IEnumerable<Dna.Ssm.VariantModel> Ssms;
    public IEnumerable<Dna.Cnv.VariantModel> Cnvs;
    public IEnumerable<Dna.Sv.VariantModel> Svs;
    public IEnumerable<Rna.GeneExpressionModel> Exps;
    public IEnumerable<ResourceModel> Resources;
}
