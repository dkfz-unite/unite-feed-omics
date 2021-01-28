namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
{
    public class Resource
    {
        public string Pid { get; set; }

        public Analysis Analysis { get; set; }
        public Sample[] Samples { get; set; }

        public void Sanitise()
        {
            Pid = Pid.Trim();

            Analysis?.Sanitise();

            if(Samples != null)
            {
                foreach (var sample in Samples)
                {
                    sample.Sanitise();
                }
            }
        }
    }
}
