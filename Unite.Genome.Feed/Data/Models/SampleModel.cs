namespace Unite.Genome.Feed.Data.Models;

public class SampleModel
{
    public string ReferenceId { get; set; }

    public double? Ploidy { get; set; }
    public double? Purity { get; set; }

    public SpecimenModel Specimen { get; set; }
}
