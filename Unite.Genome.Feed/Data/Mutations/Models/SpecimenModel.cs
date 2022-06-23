using Unite.Genome.Feed.Data.Mutations.Models.Enums;

namespace Unite.Genome.Feed.Data.Mutations.Models;

public class SpecimenModel
{
    public string ReferenceId { get; set; }
    public SpecimenType Type { get; set; }

    public DonorModel Donor { get; set; }
}
