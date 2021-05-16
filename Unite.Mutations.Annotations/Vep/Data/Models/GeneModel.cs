namespace Unite.Mutations.Annotations.Vep.Data.Models
{
    internal class GeneModel
    {
        public string Symbol { get; set; }
        public bool Strand { get; set; }
        public string Biotype { get; set; }

        public string EnsemblId { get; set; }
    }
}
