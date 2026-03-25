using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Models.Dna.Cnv;

public class ProfileModel
{
    public Chromosome Chromosome;
    public ChromosomeArm? ChromosomeArm;
    public float Gain;
    public float Loss;
    public float Neutral;
}