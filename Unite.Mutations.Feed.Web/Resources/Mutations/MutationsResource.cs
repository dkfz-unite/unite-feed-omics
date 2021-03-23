using Unite.Mutations.Feed.Web.Resources.Extensions;

namespace Unite.Mutations.Feed.Web.Resources.Mutations
{
    public class MutationsResource
    {
        public string Pid { get; set; }

        public AnalysisResource Analysis { get; set; }
        public SampleResource[] Samples { get; set; }

        public void Sanitise()
        {
            Pid = Pid.Trim();

            Analysis?.Sanitise();

            Samples?.ForEach(sample => sample.Sanitise());
        }
    }
}
