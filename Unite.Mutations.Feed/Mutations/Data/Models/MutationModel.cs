using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Mutations.Data.Models
{
    public class MutationModel
    {
        public string Code { get; set; }
        public Chromosome Chromosome { get; set; }
        public SequenceType SequenceType { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public MutationType Type { get; set; }
        public string ReferenceBase { get; set; }
        public string AlternateBase { get; set; }
    }
}