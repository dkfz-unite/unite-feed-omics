using Unite.Data.Entities.Mutations.Enums;
using Unite.Data.Utilities.Mutations;

namespace Unite.Mutations.Feed.Web.Services.Mutations
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
            var position = PositionParser.Parse(Position).Start.ToString();

            return HGVsCodeGenerator.Generate(Chromosome.Value, SequenceType.Value, position, Ref, Alt);
        }
    }
}