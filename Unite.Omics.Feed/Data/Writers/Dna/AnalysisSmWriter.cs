using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Data.Configuration;

namespace Unite.Omics.Feed.Data.Writers.Dna;

public class AnalysisSmWriter : Writers.AnalysisWriter<Models.Dna.Sm.AnalysisModel, AnalysisWriteAudit>
{
    private readonly IGenomeOptions _genomeOptions;


    private Repositories.Dna.Sm.VariantRepository _variantRepository;
    private Repositories.Dna.Sm.VariantEntryRepository _variantEntryRepository;


    public AnalysisSmWriter(IDbContextFactory<DomainDbContext> dbContextFactory, IGenomeOptions genomeOptions) : base(dbContextFactory)
    {
        _genomeOptions = genomeOptions;
    }


    protected override void InitializeEntries(DomainDbContext dbContext)
    {
        _variantRepository = new Repositories.Dna.Sm.VariantRepository(dbContext, _genomeOptions);
        _variantEntryRepository = new Repositories.Dna.Sm.VariantEntryRepository(dbContext, _variantRepository);
    }

    protected override void WriteEntries(int sampleId, Models.Dna.Sm.AnalysisModel model, ref AnalysisWriteAudit audit)
    {
        var models = model.Entries;
        if (models.IsEmpty())
            return;

        var variants = _variantRepository.CreateMissing(models);
        audit.Variants.AddRange(variants.Select(variant => variant.Id));
        audit.VariantsCreated += variants.Count();

        var removedVariants = _variantEntryRepository.RemoveAll(sampleId, model.EntriesScope);
        audit.Entries.AddRange(removedVariants);

        var variantEntries = _variantEntryRepository.CreateAll(sampleId, models, variants);
        audit.Entries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.EntriesCreated += variantEntries.Count();
    }
}
