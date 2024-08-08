using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Converters;

public class SampleModelConverter
{
    private readonly ResourceModelConverter _resourceModelConverter = new();


    public DataModels.SampleModel Convert(SampleModel sampleModel)
    {
        return new DataModels.SampleModel
        {
            Purity = sampleModel.Purity,
            Ploidy = sampleModel.Ploidy ?? SampleModel.DefaultPloidy,
            Cells = sampleModel.Cells,
            Genome = sampleModel.Genome,
            Specimen = ConvertSpecimen(sampleModel),
            Analysis = ConvertAnalysis(sampleModel),
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

    private DataModels.AnalysisModel ConvertAnalysis(SampleModel sampleModel)
    {
        return new DataModels.AnalysisModel
        {
            Type = sampleModel.AnalysisType.Value,
            Date = sampleModel.AnalysisDate,
            Day = sampleModel.AnalysisDay
        };
    }

    private DataModels.ResourceModel[] ConvertResources(ResourceModel[] resourceModels)
    {
        return resourceModels?.Select(_resourceModelConverter.Convert).ToArray();
    }
}
