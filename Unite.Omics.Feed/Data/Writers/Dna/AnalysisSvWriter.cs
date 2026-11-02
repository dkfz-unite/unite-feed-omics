using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Writers.Dna;

public class AnalysisSvWriter : Writers.AnalysisWriter<AnalysisModel<Models.Dna.Sv.VariantModel>, AnalysisWriteAudit>
{
    private readonly IGenomeOptions _genomeOptions;


    private Repositories.Dna.Sv.VariantRepository _variantRepository;
    private Repositories.Dna.Sv.VariantEntryRepository _variantEntryRepository;


    public AnalysisSvWriter(IDbContextFactory<DomainDbContext> dbContextFactory, IGenomeOptions genomeOptions) : base(dbContextFactory)
    {
        _genomeOptions = genomeOptions;
    }


    protected override void InitializeEntries(DomainDbContext dbContext)
    {
        _variantRepository = new Repositories.Dna.Sv.VariantRepository(dbContext, _genomeOptions);
        _variantEntryRepository = new Repositories.Dna.Sv.VariantEntryRepository(dbContext, _variantRepository);
    }

    protected override void WriteEntries(int sampleId, AnalysisModel<Models.Dna.Sv.VariantModel> model, ref AnalysisWriteAudit audit)
    {
        var models = model.Entries;
        if (models.IsEmpty())
            return;

        var variants = _variantRepository.CreateMissing(models);
        audit.Variants.AddRange(variants.Select(variant => variant.Id));
        audit.VariantsCreated += variants.Count();

        var removedVariants = _variantEntryRepository.RemoveAll(sampleId);
        audit.Entries.AddRange(removedVariants);

        var variantEntries = _variantEntryRepository.CreateAll(sampleId, models, variants);
        audit.Entries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.EntriesCreated += variantEntries.Count();
    }
}
