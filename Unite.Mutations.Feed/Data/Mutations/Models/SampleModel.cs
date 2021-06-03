using System;

namespace Unite.Mutations.Feed.Data.Mutations.Models
{
    public class SampleModel
    {
        public string ReferenceId { get; set; }

        public DateTime? Date { get; set; }

        public SpecimenModel Specimen { get; set; }
    }
}
