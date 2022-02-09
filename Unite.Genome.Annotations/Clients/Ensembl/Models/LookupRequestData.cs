using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Ensembl.Client.Models
{
    internal class LookupRequestData
    {
        [JsonPropertyName("ids")]
        public string[] Ids { get; set; }

        public LookupRequestData(IEnumerable<string> ensemblIds)
        {
            Ids = ensemblIds.ToArray();
        }
    }
}
