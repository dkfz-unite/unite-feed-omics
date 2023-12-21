using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Mappers;

public class SampleModelMapper
{    
    public static void Map(in SampleModel source, DataModels.SpecimenModel target)
    {
        target.ReferenceId = source.SpecimenId;
        target.Type = source.SpecimenType.Value;

        target.Donor = new DataModels.DonorModel();
        target.Donor.ReferenceId = source.DonorId;
    }
}
