using Unite.Mutations.Feed.Web.Models.Extensions;

namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public class AnalysedSampleModel : SampleModel
    {
        /// <summary>
        /// List of matched sample id
        /// </summary>
        public string[] MatchedSamples { get; set; }

        /// <summary>
        /// List of mutations appeared in the sample after analysis
        /// </summary>
        public MutationModel[] Mutations { get; set; }


        public override void Sanitise()
        {
            base.Sanitise();

            MatchedSamples?.ForEach(matchedSample => matchedSample.Trim());
            Mutations?.ForEach(mutation => mutation.Sanitise());
        }
    }
}
