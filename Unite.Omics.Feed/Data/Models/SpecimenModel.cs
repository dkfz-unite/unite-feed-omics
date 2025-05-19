using Unite.Data.Entities.Specimens.Enums;

namespace Unite.Omics.Feed.Data.Models;

public class SpecimenModel
{
    public string ReferenceId;
    public SpecimenType Type;

    public DonorModel Donor;
}
