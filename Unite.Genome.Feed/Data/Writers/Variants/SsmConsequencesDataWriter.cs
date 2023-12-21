using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Variants.SSM;

namespace Unite.Genome.Feed.Data.Writers.Variants;

public class SsmConsequencesDataWriter : ConsequencesDataWriter<AffectedTranscript, Variant, Models.Variants.SSM.VariantModel>
{
    public SsmConsequencesDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        _geneRepository = new GeneRepository(_dbContext);
        _proteinRepository = new ProteinRepository(_dbContext);
        _transcriptRepository = new TranscriptRepository(_dbContext);
        _variantRepository = new VariantRepository(_dbContext);
        _affectedTranscriptRepository = new AffectedTranscriptRepository(_dbContext, (VariantRepository)_variantRepository);
    }

    public void Refresh()
    {
        _dbContext.Dispose();
        _dbContext = _dbContextFactory.CreateDbContext();

        _geneRepository = new GeneRepository(_dbContext);
        _proteinRepository = new ProteinRepository(_dbContext);
        _transcriptRepository = new TranscriptRepository(_dbContext);
        _variantRepository = new VariantRepository(_dbContext);
        _affectedTranscriptRepository = new AffectedTranscriptRepository(_dbContext, (VariantRepository)_variantRepository);
    }
}
