using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Converters;

public abstract class SeqDataModelConverter<TEntry> where TEntry : class, new()
{
    public virtual DataModels.SampleModel Convert(SeqDataModel<TEntry> sequencingDataModel)
    {
        var sample = ConvertSample(sequencingDataModel.TargetSample);
        
        if (sequencingDataModel.MatchedSample != null)
        {
            sample.MatchedSample = ConvertSample(sequencingDataModel.MatchedSample);
        }
                
        MapEntries(sequencingDataModel, sample);

        return sample;
    }


    private static DataModels.SampleModel ConvertSample(SampleModel sampleModel)
    {
        return new DataModels.SampleModel
        {
            Purity = sampleModel.Purity,
            Ploidy = sampleModel.Ploidy,
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


    protected abstract void MapEntries(SeqDataModel<TEntry> sequencingDataModel, DataModels.SampleModel sample);
}
