using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Web.Resources.Mutations
{
    public class MutationResource
    {
        public Chromosome Chromosome { get; set; }
        public SequenceType SequenceType { get; set; }
        public string Position { get; set; }
        public string Ref { get; set; }
        public string Alt { get; set; }

        public MutationResource()
        {
            SequenceType = SequenceType.LinearGenomicDNA;
        }

        public void Sanitise()
        {
            Position = Position?.Trim();
            Ref = Ref?.Trim().ToUpper();
            Alt = Alt?.Trim().ToUpper();
        }
    }
}