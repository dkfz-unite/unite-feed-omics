using Unite.Omics.Feed.Web.Models.Base;
using SampleModel = Unite.Omics.Feed.Data.Models.SampleModel;

namespace Unite.Omics.Feed.Web.Models.Dna.CnvProfile.Converters;

public class CnvProfileModelConverter: Base.Converters.AnalysisModelConverter<CnvProfileModel>
{
    protected override void MapEntries(AnalysisModel<CnvProfileModel> source, SampleModel target)
    {
        var predicate = new Func<CnvProfileModel, bool>(model => model.Gain.HasValue || model.Loss.HasValue || model.Neutral.HasValue);

        target.CnvProfiles = source.Entries.Distinct().Where(predicate).Select(profile =>
        {
            var gain = profile.Gain ?? 0;
            var loss = profile.Loss ?? 0;
            var neutral = profile.Neutral ?? 1 - gain - loss;

            return new Data.Models.Dna.Cnv.ProfileModel
            {
                Chromosome =  profile.Chromosome.Value,
                ChromosomeArm =  profile.ChromosomeArm.Value,
                Gain = gain,
                Loss = loss,
                Neutral = neutral
            };
            
        }).ToArray();
    }
}
