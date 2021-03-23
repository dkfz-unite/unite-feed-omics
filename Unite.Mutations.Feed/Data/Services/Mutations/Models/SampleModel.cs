using System;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Data.Services.Mutations.Models
{
    public class SampleModel
    {
        public string Name { get; set; }
        public SampleType? Type { get; set; }
        public SampleSubtype? Subtype { get; set; }
        public DateTime? Date { get; set; }

        public Sample ToEntity()
        {
            var sample = new Sample
            {
                Name = Name,
                TypeId = Type,
                SubtypeId = Subtype,
                Date = Date
            };

            return sample;
        }
    }
}
