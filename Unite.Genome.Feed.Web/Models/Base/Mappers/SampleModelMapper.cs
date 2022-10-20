using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Mappers;

public class SampleModelMapper
{
    public void Map(in SampleModel source, DataModels.SampleModel target)
    {
        target.ReferenceId = source.Id;
        target.Ploidy = source.Ploidy;
        target.Purity = source.Purity;

        target.Specimen = new DataModels.SpecimenModel();
        target.Specimen.ReferenceId = source.SpecimenId;
        target.Specimen.Type = source.SpecimenType.Value;

        target.Specimen.Donor = new DataModels.DonorModel();
        target.Specimen.Donor.ReferenceId = source.DonorId;
    }
}
