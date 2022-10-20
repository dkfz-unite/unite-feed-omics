
using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Data.Services;

namespace Unite.Genome.Annotations.Data.Repositories.Variants.SSM;

internal class AffectedTranscriptRepository : AffectedTranscriptRepository<Variant, AffectedTranscript>
{
    public AffectedTranscriptRepository(DomainDbContext dbContext) : base(dbContext)
    {
    }


    protected override AffectedTranscript Convert(Models.Variants.AffectedTranscriptModel model)
    {
        var entity = base.Convert(model);

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
