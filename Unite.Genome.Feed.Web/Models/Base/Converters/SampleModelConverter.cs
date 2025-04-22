using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Converters;

public class SampleModelConverter
{
    private readonly ResourceModelConverter _resourceModelConverter = new();


    public DataModels.SampleModel Convert(SampleModel sampleModel)
    {
        return new DataModels.SampleModel
        {
            Genome = sampleModel.Genome ?? SampleModel.DefaultGenome,
            Purity = sampleModel.Purity,
            Ploidy = sampleModel.Ploidy ?? SampleModel.DefaultPloidy,
            Cells = sampleModel.Cells,
            Specimen = ConvertSpecimen(sampleModel),
            Analysis = ConvertAnalysis(sampleModel),
            Resources = ConvertResources(sampleModel.Resources)
        };
    }


    private DataModels.SpecimenModel ConvertSpecimen(SampleModel sampleModel)
    {
        Console.WriteLine($"Converting Specimen: {sampleModel.SpecimenId}, {sampleModel.SpecimenType}");
        return new DataModels.SpecimenModel
        {
            ReferenceId = sampleModel.SpecimenId,
            Type = sampleModel.SpecimenType.Value,
            Donor = new DataModels.DonorModel { ReferenceId = sampleModel.DonorId }
        };
    }

    private DataModels.AnalysisModel ConvertAnalysis(SampleModel sampleModel)
    {
        Console.WriteLine($"Converting Analysis: {sampleModel.AnalysisType}, {sampleModel.AnalysisDate}, {sampleModel.AnalysisDay}");
        return new DataModels.AnalysisModel
        {
            Type = sampleModel.AnalysisType.Value,
            Date = sampleModel.AnalysisDate,
            Day = sampleModel.AnalysisDay
        };
    }

    private DataModels.ResourceModel[] ConvertResources(ResourceModel[] resourceModels)
    {
        Console.WriteLine($"Converting Resources: {resourceModels?.Length}");
        return resourceModels?.Select(_resourceModelConverter.Convert).ToArray();
    }
}
