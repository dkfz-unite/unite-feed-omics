using Unite.Genome.Feed.Web.Models.Base.Mappers;

using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Converters;

public abstract class SequencingDataModelConverter<TEntry> where TEntry : class, new()
{
    protected readonly AnalysisModelMapper _analysisModelMapper;
    protected readonly SampleModelMapper _sampleModelMapper;


    protected SequencingDataModelConverter()
    {
        _analysisModelMapper = new AnalysisModelMapper();
        _sampleModelMapper = new SampleModelMapper();
    }


    public virtual DataModels.AnalysedSampleModel Convert(SequencingDataModel<TEntry> sequencingDataModel)
    {
        var analysedSample = new DataModels.AnalysedSampleModel
        {
            Analysis = Convert(sequencingDataModel.Analysis),
            TargetSample = Convert(sequencingDataModel.TargetSample),
            MatchedSample = Convert(sequencingDataModel.MatchedSample),
            Purity = sequencingDataModel.TargetSample.Purity,
            Ploidy = sequencingDataModel.TargetSample.Ploidy
        };
                
        MapEntries(sequencingDataModel, analysedSample);

        return analysedSample;
    }


    private static DataModels.AnalysisModel Convert(AnalysisModel analysisModel)
    {
        var analysis = new DataModels.AnalysisModel();

        AnalysisModelMapper.Map(analysisModel, analysis);

        return analysis;
    }

    private static DataModels.SpecimenModel Convert(SampleModel sampleModel)
    {
        if (sampleModel == null)
        {
            return null;
        }

        var sample = new DataModels.SpecimenModel();

        SampleModelMapper.Map(sampleModel, sample);

        return sample;
    }

    protected abstract void MapEntries(SequencingDataModel<TEntry> sequencingDataModel, DataModels.AnalysedSampleModel analysedSample);
}
