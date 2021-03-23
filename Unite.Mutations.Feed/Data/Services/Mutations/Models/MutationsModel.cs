using System.Collections.Generic;

namespace Unite.Mutations.Feed.Data.Services.Mutations.Models
{
    public class MutationsModel
    {
        public DonorModel Donor { get; set; }

        public AnalysisModel Analysis { get; set; }

        public IEnumerable<AnalysedSampleModel> AnalysedSamples { get; set; }
    }
}
