using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Converters;

public abstract class AnalysisModelConverter<TEntry> where TEntry : class, new()
{
    private readonly SampleModelConverter _sampleModelConverter = new();


    public virtual DataModels.SampleModel Convert(AnalysisModel<TEntry> analysisModel)
    {
        var sample = _sampleModelConverter.Convert(analysisModel.TargetSample);
        
        if (analysisModel.MatchedSample != null)
        {
            sample.MatchedSample = _sampleModelConverter.Convert(analysisModel.MatchedSample);
        }
                
        MapEntries(analysisModel, sample);

        return sample;
    }


    protected abstract void MapEntries(AnalysisModel<TEntry> sequencingDataModel, DataModels.SampleModel sample);
}
