using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Omics.Feed.Data.Repositories;
using Unite.Omics.Feed.Data.Repositories.Dna.Sm;

namespace Unite.Omics.Feed.Data.Writers.Dna;

public class EffectsSmWriter : EffectsWriter<AffectedTranscript, Variant, Models.Dna.Sm.VariantModel>
{
    public EffectsSmWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
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
