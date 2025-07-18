﻿using System.Text.Json.Serialization;
using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources;

public record TranscriptResource : LookupResource
{
    //[JsonPropertyName("Parent")]
    [JsonPropertyName("geneId")]
    public string GeneId { get; set; }

    //[JsonPropertyName("seq_region_name")]
    [JsonPropertyName("chromosome")]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public Chromosome Chromosome { get; set; }

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("end")]
    public int End { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("exonicLength")]
    public int? ExonicLength { get; set; }

    [JsonPropertyName("strand")]
    public bool Strand { get; set; }

    [JsonPropertyName("biotype")]
    public string Biotype { get; set; }

    //[JsonPropertyName("display_name")]
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("isCanonical")]
    public bool IsCanonical { get; set; }


    [JsonPropertyName("protein")]
    public ProteinResource Protein { get; set; }
}
