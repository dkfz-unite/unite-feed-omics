using System;
using Unite.Data.Entities.Specimens.Tissues.Enums;

namespace Unite.Mutations.Feed.Data.Mutations.Models
{
    public class TissueModel : SpecimenModel
    {
        public string ReferenceId { get; set; }

        public TissueType? Type { get; set; }
        public TumourType? TumourType { get; set; }
        public DateTime? ExtractionDate { get; set; }
        public string Source { get; set; }
    }
}