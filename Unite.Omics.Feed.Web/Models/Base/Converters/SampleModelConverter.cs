using DataModels = Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Web.Models.Base.Converters;

public class SampleModelConverter
{
    private readonly ResourceModelConverter _resourceModelConverter = new();


    public DataModels.SampleModel Convert(SampleModel sampleModel)
    {
        return new DataModels.SampleModel
        {
            Genome = sampleModel.Genome,
            Purity = sampleModel.Purity,
            Ploidy = sampleModel.Ploidy ?? SampleModel.DefaultPloidy,
            Cells = sampleModel.Cells,
            Batch = sampleModel.Batch,
            Specimen = ConvertSpecimen(sampleModel),
            Assay = ConvertAssay(sampleModel),
            Resources = ConvertResources(sampleModel.Resources)
        };
    }


    private DataModels.SpecimenModel ConvertSpecimen(SampleModel sampleModel)
    {
        return new DataModels.SpecimenModel
        {
            ReferenceId = sampleModel.SpecimenId,
            Type = sampleModel.SpecimenType.Value,
            Donor = new DataModels.DonorModel { ReferenceId = sampleModel.DonorId }
        };
    }

    private DataModels.AssayModel ConvertAssay(SampleModel sampleModel)
    {
        return new DataModels.AssayModel
        {
            Type = sampleModel.AssayType.Value,
            Date = sampleModel.AssayDate,
            Day = sampleModel.AssayDay
        };
    }

    private DataModels.ResourceModel[] ConvertResources(ResourceModel[] resourceModels)
    {
        return resourceModels?.Select(_resourceModelConverter.Convert).ToArray();
    }
}
