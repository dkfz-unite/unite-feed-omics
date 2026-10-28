using DataModels = Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Web.Models.Base.Converters;

public class AnalysisModelConverter<TEntry, TModel>
    where TEntry : class, new()
    where TModel : DataModels.AnalysisModel, new()
{
    private readonly SampleModelConverter _sampleModelConverter = new();
    private readonly ResourceModelConverter _resourceModelConverter = new();


    public virtual TModel Convert(AnalysisModel<TEntry> source)
    {
        var model = new TModel
        {
            TargetSample = _sampleModelConverter.Convert(source.TargetSample),
            MatchedSample = source.MatchedSample == null ? null : _sampleModelConverter.Convert(source.MatchedSample),
            Resources = source.Resources?.Select(_resourceModelConverter.Convert).ToArray()
        };

        MapEntries(source, model);
        return model;
    }


    protected virtual void MapEntries(AnalysisModel<TEntry> source, TModel target)
    {
    }
}
