using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Converters;

public class SampleModelConverter
{
    public DataModels.SampleModel Convert(SampleModel sampleModel)
    {
        return new DataModels.SampleModel
        {
            Purity = sampleModel.Purity,
            Ploidy = sampleModel.Ploidy ?? SampleModel.DefaultPloidy,
            CellsNumber = sampleModel.CellsNumber,
            GenesModel = sampleModel.GenesModel,
            Specimen = ConvertSpecimen(sampleModel),
            Analysis = ConvertAnalysis(sampleModel),
            Resources = sampleModel.Resources?.Select(ConvertResource).ToArray(),
        };
    }


    private static DataModels.SpecimenModel ConvertSpecimen(SampleModel sampleModel)
    {
        return new DataModels.SpecimenModel
        {
            ReferenceId = sampleModel.SpecimenId,
            Type = sampleModel.SpecimenType.Value,
            Donor = new DataModels.DonorModel { ReferenceId = sampleModel.DonorId }
        };
    }

    private static DataModels.AnalysisModel ConvertAnalysis(SampleModel sampleModel)
    {
        return new DataModels.AnalysisModel
        {
            Type = sampleModel.AnalysisType.Value,
            Date = sampleModel.AnalysisDate,
            Day = sampleModel.AnalysisDay
        };
    }

    private static DataModels.ResourceModel ConvertResource(ResourceModel resourceModel)
    {
        return new DataModels.ResourceModel
        {
            Type = resourceModel.Type,
            Format = resourceModel.Format,
            Url = resourceModel.Url
        };
    }
}
