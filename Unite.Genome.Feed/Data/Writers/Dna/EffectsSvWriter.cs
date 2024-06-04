using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Sv;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Dna.Sv;

namespace Unite.Genome.Feed.Data.Writers.Dna;

public class EffectsSvWriter : EffectsWriter<AffectedTranscript, Variant, Models.Dna.Sv.VariantModel>
{
    public EffectsSvWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _geneRepository = new GeneRepository(dbContext);
        _proteinRepository = new ProteinRepository(dbContext);
        _transcriptRepository = new TranscriptRepository(dbContext);
        _variantRepository = new VariantRepository(dbContext);
        _affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext, (VariantRepository)_variantRepository);
    }
}
