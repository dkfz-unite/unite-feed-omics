using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Models.Dna.Cnv;

public class ProfileModel
{
    public Chromosome Chromosome { get; set; }
    public ChromosomeArm ChromosomeArm { get; set; }
    public float Gain { get; set; }
    public float Loss { get; set; }
    public float Neutral { get; set; }
}