using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Ensembl.Models;

internal class LookupRequestData
{
    [JsonPropertyName("ids")]
    public string[] Ids { get; set; }

    public LookupRequestData(IEnumerable<string> ensemblIds)
    {
        Ids = ensemblIds.ToArray();
    }
}
