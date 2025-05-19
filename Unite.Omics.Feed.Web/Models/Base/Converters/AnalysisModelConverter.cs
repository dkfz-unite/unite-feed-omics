using Unite.Essentials.Extensions;
using DataModels = Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Web.Models.Base.Converters;

public abstract class AnalysisModelConverter<TEntry> where TEntry : class, new()
{
    private readonly SampleModelConverter _sampleModelConverter = new();
    private readonly ResourceModelConverter _resourceModelConverter = new();


    public virtual DataModels.SampleModel Convert(AnalysisModel<TEntry> analysisModel)
    {
        var sample = _sampleModelConverter.Convert(analysisModel.TargetSample);
        
        if (analysisModel.MatchedSample != null)
        {
            sample.MatchedSample = _sampleModelConverter.Convert(analysisModel.MatchedSample);
        }

        MapResources(analysisModel, sample);  
        MapEntries(analysisModel, sample);
        
        return sample;
    }


    protected virtual void MapResources(AnalysisModel<TEntry> sequencingDataModel, DataModels.SampleModel sample)
    {
        if (sequencingDataModel.Resources.IsNotEmpty())
        {
            sample.Resources = Enumerable.Concat
            (
                sample.Resources ?? [],
                sequencingDataModel.Resources.Select(_resourceModelConverter.Convert)
            )
            .ToArray();
        }
    }

    protected abstract void MapEntries(AnalysisModel<TEntry> sequencingDataModel, DataModels.SampleModel sample);
}
