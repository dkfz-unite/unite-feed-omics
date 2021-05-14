using Unite.Data.Entities.Specimens.Tissues.Enums;

namespace Unite.Mutations.Feed.Mutations.Data.Models
{
    public class TissueModel
    {
        public TissueType Type { get; set; }
        public TumourType? TumourType { get; set; }
    }
}