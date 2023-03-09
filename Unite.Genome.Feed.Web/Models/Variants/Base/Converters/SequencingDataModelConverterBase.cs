using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Base.Mappers;

using DataModels = Unite.Genome.Feed.Data.Models;

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


    public virtual DataModels.AnalysisModel Convert(SequencingDataModel<TModel> sequencingDataModel)
    {
        var analysis = Convert(sequencingDataModel.Analysis);

        analysis.AnalysedSamples = sequencingDataModel.Samples.Select(analysedSampleModel =>
        {
            var analysedSample = new DataModels.AnalysedSampleModel();

            analysedSample.AnalysedSample = Convert(analysedSampleModel);

            if (!string.IsNullOrEmpty(analysedSampleModel.MatchedSampleId))
            {
                var matchedAnalysedSampleModel = sequencingDataModel.FindSample(analysedSampleModel.MatchedSampleId);

                analysedSample.MatchedSample = Convert(matchedAnalysedSampleModel);
            }

            if (analysedSampleModel.Variants != null)
            {
                MapVariants(analysedSampleModel, analysedSample);
            }

            return analysedSample;

        }).ToArray();

        return analysis;
    }


    private DataModels.AnalysisModel Convert(AnalysisModel analysisModel)
    {
        var analysis = new DataModels.AnalysisModel();

        if (analysisModel != null)
        {
            _analysisModelMapper.Map(analysisModel, analysis);
        }

        return analysis;
    }

    private DataModels.SampleModel Convert(AnalysedSampleModel<TModel> analysedSampleModel)
    {
        var sample = new DataModels.SampleModel();

        _sampleModelMapper.Map(analysedSampleModel, sample);

        return sample;
    }

    protected abstract void MapVariants(AnalysedSampleModel<TModel> analysedSampleModel, DataModels.AnalysedSampleModel analysedSample);
}
