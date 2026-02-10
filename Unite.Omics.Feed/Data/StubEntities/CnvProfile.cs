using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.StubEntities;

//TODO: define real entity in Unite.Data package
public class CnvProfile
{
    public int SampleId { get; set; }
    public Chromosome Chromosome { get; set; }
    public ChromosomeArm ChromosomeArm { get; set; }
    public float Gain { get; set; }
    public float Loss { get; set; }
    public float Neutral { get; set; }
}