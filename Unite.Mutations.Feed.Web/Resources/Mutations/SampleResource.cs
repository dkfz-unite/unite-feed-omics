using System;
using System.Linq;
using Unite.Data.Entities.Mutations.Enums;
using Unite.Mutations.Feed.Web.Resources.Extensions;

namespace Unite.Mutations.Feed.Web.Resources.Mutations
{
    public class SampleResource
    {
        public string Name { get; set; }
        public SampleType? Type { get; set; }
        public SampleSubtype? Subtype { get; set; }
        public DateTime? Date { get; set; }
        public string[] MatchedSamples { get; set; }

        public MutationResource[] Mutations { get; set; }

        public void Sanitise()
        {
            Name = Name?.Trim();

            MatchedSamples?.ForEach(matchedSample => matchedSample.Trim());
            Mutations?.ForEach(mutation => mutation.Sanitise());
        }
    }
}