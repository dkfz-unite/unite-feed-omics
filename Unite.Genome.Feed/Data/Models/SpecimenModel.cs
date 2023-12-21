using Unite.Data.Entities.Specimens.Enums;

namespace Unite.Genome.Feed.Data.Models;

public class SpecimenModel
{
    public string ReferenceId { get; set; }
    public SpecimenType Type { get; set; }

    public DonorModel Donor { get; set; }
}
