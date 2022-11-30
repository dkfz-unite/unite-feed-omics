using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models.Variants;

namespace Unite.Genome.Annotations.Data.Repositories.Variants.SSM;

internal class AffectedTranscriptRepository : AffectedTranscriptRepository<Variant, AffectedTranscript>
{
    public AffectedTranscriptRepository(DomainDbContext dbContext) : base(dbContext)
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
