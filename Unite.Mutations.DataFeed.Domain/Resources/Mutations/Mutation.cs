using System;
using Unite.Data.Entities.Extensions;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
{
    public class Mutation
    {
        public string Id { get; set; }
        public Chromosome? Chromosome { get; set; }
        public string Contig { get; set; }
        public SequenceType? SequenceType { get; set; }
        public int? Position { get; set; }
        public string Ref { get; set; }
        public string Alt { get; set; }

        public Gene Gene { get; set; }

        public string Code => GetMutationCode();
        public MutationType Type => GetMutationType();

        public void Sanitise()
        {
            Id = Id?.Trim();
            Contig = Contig?.Trim();
            Ref = Ref?.Trim().ToUpper();
            Alt = Alt?.Trim().ToUpper();

            Gene?.Sanitise();
        }

        private string GetMutationCode()
        {
            var strand = Chromosome != null
                       ? $"chr{Chromosome.ToDefinitionString()}"
                       : Contig;

            var sequenceType = SequenceType.ToDefinitionString();

            var position = Position.ToString();

            var referenceBase = !string.IsNullOrEmpty(Ref)
                                ? Ref
                                : "-";

            var alternateBase = !string.IsNullOrEmpty(Alt)
                                ? Alt
                                : "-";

            return $"{strand}:{sequenceType}.{position}{referenceBase}>{alternateBase}";
        }

        private MutationType GetMutationType()
        {
            if(!string.IsNullOrWhiteSpace(Ref) && !string.IsNullOrWhiteSpace(Alt))
            {
                if (Ref.Length == 1 && Alt.Length == 1)
                {
                    return MutationType.SNV;
                }
                else if (Ref.Length == 1 && Alt.Length > 1)
                {
                    return MutationType.INS;
                }
                else if (Ref.Length > 1 && Alt.Length == 1)
                {
                    return MutationType.DEL;
                }
                else if (Ref.Length > 1 && Alt.Length > 1)
                {
                    return MutationType.MNV;
                }
            }
            else if(string.IsNullOrWhiteSpace(Ref) && !string.IsNullOrWhiteSpace(Alt))
            {
                return MutationType.INS;
            }
            else if(!string.IsNullOrWhiteSpace(Ref) && string.IsNullOrWhiteSpace(Alt))
            {
                return MutationType.DEL;
            }

            throw new NotImplementedException($"Can't detect mutation type: REF - '{Ref}', ALT - '{Alt}'");
        }
    }
}