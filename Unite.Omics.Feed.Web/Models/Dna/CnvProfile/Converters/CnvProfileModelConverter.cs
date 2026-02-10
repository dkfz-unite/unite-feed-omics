using Unite.Omics.Feed.Web.Models.Base;
using SampleModel = Unite.Omics.Feed.Data.Models.SampleModel;

namespace Unite.Omics.Feed.Web.Models.Dna.CnvProfile.Converters;

public class CnvProfileModelConverter: Base.Converters.AnalysisModelConverter<CnvProfileModel>
{
    protected override void MapEntries(AnalysisModel<CnvProfileModel> cnvProfileSubmission, SampleModel sampleModel)
    {
        sampleModel.CnvProfiles = cnvProfileSubmission.Entries.Distinct().Select(submissionEntry =>
        {
            var model = new Unite.Omics.Feed.Data.Models.CnvProfile.CnvProfileModel
            {
                Chromosome =  submissionEntry.Chromosome,
                ChromosomeArm =  submissionEntry.ChromosomeArm,
                Gain = submissionEntry.Gain,
                Loss = submissionEntry.Loss,
                Neutral = submissionEntry.Neutral
            };
            
            return model;
        }).ToArray();
    }
}