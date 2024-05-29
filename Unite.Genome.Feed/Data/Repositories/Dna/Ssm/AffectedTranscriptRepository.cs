using Unite.Data.Context;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using Unite.Genome.Feed.Data.Models.Dna;

namespace Unite.Genome.Feed.Data.Repositories.Dna.Ssm;

public class AffectedTranscriptRepository : AffectedTranscriptRepository<AffectedTranscript, Variant, Models.Dna.Ssm.VariantModel>
{
    public AffectedTranscriptRepository(DomainDbContext dbContext, VariantRepository variantRepository) : base(dbContext, variantRepository)
    {
    }

    protected override AffectedTranscript Convert(
        AffectedTranscriptModel model,
        IEnumerable<Variant> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var entity = base.Convert(model, variantsCache, transcriptsCache);

        entity.Distance = model.Distance;

        entity.CDNAStart = model.CDNAStart;
        entity.CDNAEnd = model.CDNAEnd;
        entity.CDSStart = model.CDSStart;
        entity.CDSEnd = model.CDSEnd;
        entity.ProteinStart = model.ProteinStart;
        entity.ProteinEnd = model.ProteinEnd;

        entity.AminoAcidChange = model.AminoAcidChange;
        entity.CodonChange = model.CodonChange;

        return entity;
    }
}
