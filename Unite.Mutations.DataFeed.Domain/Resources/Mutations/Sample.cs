using System;
using System.Linq;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
{
    public class Sample
    {
        public string Name { get; set; }
        public SampleType? Type { get; set; }
        public SampleSubtype? Subtype { get; set; }
        public DateTime? Date { get; set; }

        public string[] MatchedSamples { get; set; }
        public Mutation[] Mutations { get; set; }

        public void Sanitise()
        {
            Name = Name?.Trim();

            if(MatchedSamples != null)
            {
                MatchedSamples = MatchedSamples
                    .Select(sample => sample.Trim())
                    .ToArray();
            }

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
