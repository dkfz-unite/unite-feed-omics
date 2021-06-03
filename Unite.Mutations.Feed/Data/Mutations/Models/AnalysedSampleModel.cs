using System.Collections.Generic;

namespace Unite.Mutations.Feed.Data.Mutations.Models
{
    public class AnalysedSampleModel : SampleModel
    {
        public IEnumerable<SampleModel> MatchedSamples { get; set; }

        public IEnumerable<MutationModel> Mutations { get; set; }
    }
}
