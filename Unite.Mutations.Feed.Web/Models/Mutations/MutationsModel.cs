using System.Linq;
using Unite.Mutations.Feed.Web.Models.Extensions;

namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public class MutationsModel
    {
        public AnalysisModel Analysis { get; set; }

        public AnalysedSampleModel[] Samples { get; set; }


        public AnalysedSampleModel FindSample(string id)
        {
            return Samples.FirstOrDefault(analysedSample => analysedSample.Id == id);
        }

        public void Sanitise()
        {
            Analysis?.Sanitise();

            Samples?.ForEach(sample => sample.Sanitise());
        }
    }
}
