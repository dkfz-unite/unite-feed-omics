using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Utilities.Mutations;

namespace Unite.Genome.Feed.Web.Services.Mutations
{
    public class MutationModel
    {
        public Chromosome? Chromosome { get; set; }
        public string Position { get; set; }
        public string Ref { get; set; }
        public string Alt { get; set; }


        public void Sanitise()
        {
            Position = Position?.Trim();
            Ref = Ref?.Trim().ToUpper();
            Alt = Alt?.Trim().ToUpper();
        }

        public string GetCode()
        {
            return HGVsCodeGenerator.Generate(Chromosome.Value, Position, Ref, Alt);
        }
    }
}
