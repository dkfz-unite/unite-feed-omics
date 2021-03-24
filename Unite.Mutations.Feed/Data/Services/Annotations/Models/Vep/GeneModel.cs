using Unite.Data.Entities.Mutations;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
{
    public class GeneModel
    {
        public string Symbol { get; set; }
        public bool Strand { get; set; }
        public string Biotype { get; set; }

        public string EnsemblId { get; set; }

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
