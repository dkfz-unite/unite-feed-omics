using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
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

        public Mutation ToEntity()
        {
            var mutation = new Mutation
            {
                Code = Code,
                ChromosomeId = Chromosome,
                SequenceTypeId = SequenceType,
                Start = Start,
                End = End,
                TypeId = Type,
                ReferenceBase = ReferenceBase,
                AlternateBase = AlternateBase
            };

            return mutation;
        }
    }
}
