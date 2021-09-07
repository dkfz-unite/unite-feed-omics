using System.Linq;
using Unite.Data.Extensions;

namespace Unite.Mutations.Feed.Web.Services.Mutations
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
