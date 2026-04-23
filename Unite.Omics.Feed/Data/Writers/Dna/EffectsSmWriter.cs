using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Repositories;
using Unite.Omics.Feed.Data.Repositories.Dna.Sm;

namespace Unite.Omics.Feed.Data.Writers.Dna;

public class EffectsSmWriter : EffectsWriter<AffectedTranscript, Variant, Models.Dna.Sm.VariantModel>
{    
    public EffectsSmWriter(IDbContextFactory<DomainDbContext> dbContextFactory, IGenomeOptions genomeOptions) : base(dbContextFactory, genomeOptions)
    {
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _geneRepository = new GeneRepository(dbContext, _genomeOptions);
        _proteinRepository = new ProteinRepository(dbContext, _genomeOptions);
        _transcriptRepository = new TranscriptRepository(dbContext, _genomeOptions);
        _variantRepository = new VariantRepository(dbContext, _genomeOptions);
        _affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext, _genomeOptions, (VariantRepository)_variantRepository);
    }
}
