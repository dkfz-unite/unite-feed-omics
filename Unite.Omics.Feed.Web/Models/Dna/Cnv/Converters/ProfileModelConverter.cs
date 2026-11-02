using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Models.Dna.Cnv.Converters;

public class ProfileModelConverter : Base.Converters.AnalysisModelConverter<ProfileModel, Data.Models.AnalysisModel<Data.Models.Dna.Cnv.ProfileModel>>
{
    protected override void MapEntries(AnalysisModel<ProfileModel> source, Data.Models.AnalysisModel<Data.Models.Dna.Cnv.ProfileModel> target)
    {
        var predicate = new Func<ProfileModel, bool>(model => model.Gain.HasValue || model.Loss.HasValue || model.Neutral.HasValue);

        target.Entries = source.Entries.Distinct().Where(predicate).Select(profile =>
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
