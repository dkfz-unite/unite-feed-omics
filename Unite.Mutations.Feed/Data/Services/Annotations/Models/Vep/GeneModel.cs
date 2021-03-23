using Unite.Data.Entities.Mutations;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
{
    public class GeneModel
    {
        public string Symbol;
        public bool Strand;
        public string Biotype;

        public string EnsemblId;

        public Gene ToEntity()
        {
            var gene = new Gene
            {
                Symbol = Symbol,
                Strand = Strand,

                Biotype = GetBiotype(Biotype),
                Info = GetInfo(EnsemblId)
            };

            return gene;
        }

        private Biotype GetBiotype(string biotype)
        {
            if (biotype == null)
            {
                return null;
            }

            return new Biotype
            {
                Value = biotype
            };
        }

        private GeneInfo GetInfo(string ensemblId)
        {
            if (ensemblId == null)
            {
                return null;
            }

            return new GeneInfo
            {
                EnsemblId = ensemblId
            };
        }
    }
}
