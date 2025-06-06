using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Omics.Feed.Data.Models.Dna;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Sm;

public class AffectedTranscriptRepository : AffectedTranscriptRepository<AffectedTranscript, Variant, Models.Dna.Sm.VariantModel>
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
        entity.AAStart = model.AAStart;
        entity.AAEnd = model.AAEnd;

        entity.ProteinChange = model.ProteinChange;
        entity.CodonChange = model.CodonChange;

        return entity;
    }
}
