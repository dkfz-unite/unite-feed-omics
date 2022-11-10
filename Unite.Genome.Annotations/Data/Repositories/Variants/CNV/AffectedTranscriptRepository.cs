using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models.Variants;

namespace Unite.Genome.Annotations.Data.Repositories.Variants.CNV;

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

        entity.OverlapBpNumber = model.OverlapBpNumber;
        entity.OverlapPercentage = model.OverlapPercentage;
        entity.Distance = model.Distance;

        return entity;
    }
}
