using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Web.Models.Dna.Cnvp;

public class CnvProfileModel
{
    public Chromosome Chromosome { get; set; }
    public ChromosomeArm Arm { get; set; }
    public float Gain { get; set; }
    public float Loss { get; set; }
    public float Neutral { get; set; }
}