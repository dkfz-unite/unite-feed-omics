using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
{
    public class Mutation
    {
        public string Pid { get; set; }
        public string Id { get; set; }
        public Chromosome? Chromosome { get; set; }
        public string Contig { get; set; }
        public SequenceType? SequenceType { get; set; }
        public int? Position { get; set; }
        public MutationType? Type { get; set; }
        public string Ref { get; set; }
        public string Alt { get; set; }
        public string Quality { get; set; }
        public string Filter { get; set; }
        public string Info { get; set; }

        public Gene Gene { get; set; }
        public Sample Sample { get; set; }


        public void Sanitise()
        {
            Pid = Pid?.Trim();
            Id = Id?.Trim();
            Contig = Contig?.Trim();
            Ref = Ref?.Trim().ToUpper();
            Alt = Ref?.Trim().ToUpper();
            Quality = Quality?.Trim();
            Filter = Filter?.Trim();
            Info = Info?.Trim();

            Gene?.Sanitise();
            Sample?.Sanitise();
        }
    }
}
