using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Dna.Cnv;

namespace Unite.Genome.Feed.Data.Writers.Dna;

public class EffectsCnvWriter : EffectsWriter<AffectedTranscript, Variant, Models.Dna.Cnv.VariantModel>
{
    public EffectsCnvWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
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
