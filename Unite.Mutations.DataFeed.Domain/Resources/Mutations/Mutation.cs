using System;
using Unite.Data.Entities.Extensions;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
{
    public class Mutation
    {
        public Chromosome Chromosome { get; set; }
        public SequenceType SequenceType { get; set; }
        public string Position { get; set; }
        public string Ref { get; set; }
        public string Alt { get; set; }

        public string Code => GetMutationCode();
        public MutationType Type => GetMutationType();
        public int Start => GetStart();
        public int End => GetEnd();


        public Mutation()
        {
            SequenceType = SequenceType.LinearGenomicDNA;
        }


        public void Sanitise()
        {
            Position = Position?.Trim();
            Ref = Ref?.Trim().ToUpper();
            Alt = Alt?.Trim().ToUpper();
        }


        private int GetStart()
        {
            if (Position.Contains('-'))
            {
                return int.Parse(Position.Split('-')[0]);
            }
            else
            {
                return int.Parse(Position);
            }
        }

        private int GetEnd()
        {
            if (Position.Contains('-'))
            {
                return int.Parse(Position.Split('-')[1]);
            }
            else
            {
                return int.Parse(Position);
            }
        }

        private string GetMutationCode()
        {
            var chromosome = $"chr{Chromosome.ToDefinitionString()}";

            var sequenceType = SequenceType.ToDefinitionString();

            var position = Position.ToString();

            var referenceBase = !string.IsNullOrEmpty(Ref) ? Ref : "-";

            var alternateBase = !string.IsNullOrEmpty(Alt) ? Alt : "-";

            return $"{chromosome}:{sequenceType}.{position}{referenceBase}>{alternateBase}";
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