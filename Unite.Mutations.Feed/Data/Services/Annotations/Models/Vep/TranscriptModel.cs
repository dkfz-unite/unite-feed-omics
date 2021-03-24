using Unite.Data.Entities.Mutations;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
{
    public class TranscriptModel
    {
        public string Biotype { get; set; }
        public bool Strand { get; set; }

        public string EnsemblId { get; set; }


        public Transcript ToEntity()
        {
            var transcript = new Transcript
            {
                Strand = Strand,

                Biotype = GetBiotype(Biotype),
                Info = GetInfo(EnsemblId)
            };

            return transcript;
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

        private TranscriptInfo GetInfo(string ensemblId)
        {
            if (ensemblId == null)
            {
                return null;
            }

            return new TranscriptInfo
            {
                EnsemblId = ensemblId
            };
        }
    }
}
