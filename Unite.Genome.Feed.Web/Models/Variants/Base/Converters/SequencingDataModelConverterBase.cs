using Unite.Genome.Feed.Web.Models.Base.Mappers;

using DataModels = Unite.Genome.Feed.Data.Models;
using WebModels = Unite.Genome.Feed.Web.Models;

namespace Unite.Genome.Feed.Web.Models.Variants.Base.Converters;

public abstract class SequencingDataModelConverterBase<TModel> where TModel : class, new()
{
    protected readonly AnalysisModelMapper _analysisModelMapper;
    protected readonly SampleModelMapper _sampleModelMapper;


    protected SequencingDataModelConverterBase()
    {
        _analysisModelMapper = new AnalysisModelMapper();
        _sampleModelMapper = new SampleModelMapper();
    }


    public virtual DataModels.AnalysisModel Convert(WebModels.Variants.SequencingDataModel<TModel> source)
    {
        var target = new DataModels.AnalysisModel();

        if (source.Analysis != null)
        {
            // Mapping analysis data
            _analysisModelMapper.Map(source.Analysis, target);
        }

        target.AnalysedSamples = source.Samples.Select(analysedSample =>
        {
            // Mapping analysed sample data
            var analysedSampleModel = new DataModels.AnalysedSampleModel();

            analysedSampleModel.AnalysedSample = new DataModels.SampleModel();

            _sampleModelMapper.Map(analysedSample, analysedSampleModel.AnalysedSample);

            if (!string.IsNullOrWhiteSpace(analysedSample.MatchedSampleId))
            {
                // Mapping matched sample data
                var matchedSampleId = analysedSample.MatchedSampleId;

                var matchedAnalysedSample = source.Samples.Single(analysedSample => analysedSample.Id == matchedSampleId);

                analysedSampleModel.MatchedSample = new DataModels.SampleModel();

                _sampleModelMapper.Map(matchedAnalysedSample, analysedSampleModel.MatchedSample);
            }

            if (analysedSample.Variants != null)
            {
                //Mapping variants data
                MapVariants(analysedSample, analysedSampleModel);
            }

            return analysedSampleModel;

        }).ToArray();

        return target;
    }


    /// <summary>
    /// Maps required variants data to analysed sample model
    /// </summary>
    /// <param name="analysedSampleModel"></param>
    protected abstract void MapVariants(WebModels.Variants.AnalysedSampleModel<TModel> source, DataModels.AnalysedSampleModel target);
}
