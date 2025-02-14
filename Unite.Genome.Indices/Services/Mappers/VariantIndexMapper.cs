using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Indices.Entities.Basic.Genome.Variants;
using Unite.Indices.Entities.Basic.Genome;
using Unite.Data.Entities.Genome;
using Unite.Essentials.Extensions;

namespace Unite.Genome.Indices.Services.Mappers;

public abstract class VariantIndexMapper
{
    protected static EffectIndex[] CreateFrom(in IEnumerable<Effect> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new EffectIndex
            {
                Type = entity.Type,
                Impact = entity.Impact,
                Severity = entity.Severity
            };

        }).ToArray();
    }

    protected static GeneIndex CreateFrom(in Gene entity)
    {
        return GeneIndexMapper.CreateFrom<GeneIndex>(entity);
    }

    protected static TranscriptIndex CreateFrom(in Transcript entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new TranscriptIndex
        {
            Id = entity.Id,
            StableId = entity.StableId,
            Symbol = entity.Symbol,
            Description = entity.Description,
            Biotype = entity.Biotype,
            IsCanonical = entity.IsCanonical,
            Chromosome = entity.ChromosomeId.ToDefinitionString(),
            Start = entity.Start,
            End = entity.End,
            Strand = entity.Strand,
            ExonicLength = entity.ExonicLength,

            Protein = CreateFrom(entity.Protein)
        };
    }

    protected static ProteinIndex CreateFrom(in Protein entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new ProteinIndex
        {
            Id = entity.Id,
            StableId = entity.StableId,
            IsCanonical = entity.IsCanonical,
            Start = entity.Start,
            End = entity.End,
            Length = entity.Length
        };
    }
}
