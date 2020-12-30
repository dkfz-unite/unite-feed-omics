using System;
using Unite.Data.Entities.Samples.Enums;

namespace Unite.Mutations.DataFeed.Domain.Resources.Samples
{
    public class Sample
    {
        public string Pid { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public SampleType? Type { get; set; }
        public SampleSubtype? Subtype { get; set; }
        public DateTime? Date { get; set; }

        public Mutation[] Mutations { get; set; }


        public void Sanitise()
        {
            Pid = Pid?.Trim();
            Name = Name?.Trim();
            Link = Link?.Trim();

            if (Mutations != null)
            {
                foreach (var mutation in Mutations)
                {
                    mutation.Sanitise();
                }
            }
        }
    }
}
