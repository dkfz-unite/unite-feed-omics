using System.Collections.Generic;

namespace Unite.Mutations.Feed.Data.Services.Mutations.Models
{
    public class AnalysedSampleModel : SampleModel
    {
        public IEnumerable<MatchedSampleModel> MatchedSamples { get; set; }
        public IEnumerable<MutationModel> Mutations { get; set; }
    }
}
