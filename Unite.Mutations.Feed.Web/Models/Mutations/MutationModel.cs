using Unite.Data.Entities.Mutations.Enums;
using Unite.Data.Extensions;

namespace Unite.Mutations.Feed.Web.Models.Mutations
{
    public class MutationModel
    {
        public Chromosome? Chromosome { get; set; }
        public SequenceType? SequenceType { get; set; }
        public string Position { get; set; }
        public string Ref { get; set; }
        public string Alt { get; set; }

        public MutationModel()
        {
            SequenceType = Unite.Data.Entities.Mutations.Enums.SequenceType.LinearGenomicDNA;
        }

        public void Sanitise()
        {
            Position = Position?.Trim();
            Ref = Ref?.Trim().ToUpper();
            Alt = Alt?.Trim().ToUpper();
        }

        public string GetCode()
        {
            var chromosome = $"chr{Chromosome.ToDefinitionString()}";
            var sequenceType = SequenceType.ToDefinitionString();
            var position = Position;
            var referenceBase = Ref ?? "-";
            var alternateBase = Alt ?? "-";

            return $"{chromosome}:{sequenceType}.{position}{referenceBase}>{alternateBase}";
        }
    }
}