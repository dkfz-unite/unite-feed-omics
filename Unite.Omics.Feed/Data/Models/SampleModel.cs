using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;

namespace Unite.Omics.Feed.Data.Models;

public record SampleModel
{
    public string Genome;
    public double? Purity;
    public double? Ploidy;
    public int? Cells;
    public string Batch;

    public SpecimenModel Specimen;
    public AnalysisModel Analysis;
    public SampleModel MatchedSample;
    
    public IEnumerable<Dna.Sm.VariantModel> Sms;
    public IEnumerable<Dna.Cnv.VariantModel> Cnvs;
    public IEnumerable<Dna.Sv.VariantModel> Svs;
    public IEnumerable<Rna.GeneExpressionModel> GeneExpressions;
    public IEnumerable<Prot.ProteinExpressionModel> ProteinExpressions;
    public IEnumerable<ResourceModel> Resources;
    public IEnumerable<Dna.Cnv.ProfileModel> CnvProfiles;

    public SmType[] SmScope;
}
