using Unite.Genome.Feed.Web.Models.Base.Mappers;

using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Transcriptomics.Converters;

public class TranscriptomicsDataModelConverter
{
    protected readonly AnalysisModelMapper _analysisModelMapper;
    protected readonly SampleModelMapper _sampleModelMapper;


    public TranscriptomicsDataModelConverter()
	{
        _analysisModelMapper = new AnalysisModelMapper();
        _sampleModelMapper = new SampleModelMapper();
    }


    public virtual DataModels.AnalysisModel Convert(TranscriptomicsDataModel transcriptomicsDataModel)
    {
        var analysis = Convert(transcriptomicsDataModel.Analysis);

        var analysedSample = new DataModels.AnalysedSampleModel();

        analysedSample.AnalysedSample = Convert(transcriptomicsDataModel.Sample);

        analysis.AnalysedSamples = new DataModels.AnalysedSampleModel[] { analysedSample };

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

    private DataModels.SampleModel Convert(AnalysedSampleModel analysedSampleModel)
    {
        var sample = new DataModels.SampleModel();

        _sampleModelMapper.Map(analysedSampleModel, sample);

        return sample;
    }
}
