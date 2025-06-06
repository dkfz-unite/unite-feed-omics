using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Omics.Feed.Data.Models.Dna;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Sv;

public class AffectedTranscriptRepository : AffectedTranscriptRepository<AffectedTranscript, Variant, Models.Dna.Sv.VariantModel>
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

        entity.OverlapBpNumber = model.OverlapBpNumber;
        entity.OverlapPercentage = model.OverlapPercentage;
        entity.Distance = model.Distance;

        entity.CDNAStart = model.CDNAStart;
        entity.CDNAEnd = model.CDNAEnd;
        entity.CDSStart = model.CDSStart;
        entity.CDSEnd = model.CDSEnd;
        entity.AAStart = model.AAStart;
        entity.AAEnd = model.AAEnd;

        return entity;
    }
}
