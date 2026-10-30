using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories.Constants;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Writers.Dna;

public class AnalysisCnvWriter : Writers.AnalysisWriter<AnalysisModel<Models.Dna.Cnv.VariantModel>, AnalysisWriteAudit>
{
    private readonly IGenomeOptions _genomeOptions;


    private Repositories.Dna.Cnv.VariantRepository _variantRepository;
    private Repositories.Dna.Cnv.VariantEntryRepository _variantEntryRepository;


    public AnalysisCnvWriter(IDbContextFactory<DomainDbContext> dbContextFactory, IGenomeOptions genomeOptions) : base(dbContextFactory)
    {
        _genomeOptions = genomeOptions;
    }


    protected override void InitializeEntries(DomainDbContext dbContext)
    {
        _variantRepository = new Repositories.Dna.Cnv.VariantRepository(dbContext, _genomeOptions);
        _variantEntryRepository = new Repositories.Dna.Cnv.VariantEntryRepository(dbContext, _variantRepository);
    }

    protected override void WriteEntries(int sampleId, AnalysisModel<Models.Dna.Cnv.VariantModel> model, ref AnalysisWriteAudit audit)
    {
        var models = model.Entries;
        if (models.IsEmpty())
            return;

        var variants = _variantRepository.CreateMissing(models);
        //TODO: compile only once
        var predicate = Predicates.IsInfluentCnv.Compile();
        audit.Variants.AddRange(variants.Where(predicate).Select(variant => variant.Id));
        audit.VariantsCreated += variants.Count();

        var removedVariants = _variantEntryRepository.RemoveAll(sampleId);
        audit.Entries.AddRange(removedVariants);

        var variantEntries = _variantEntryRepository.CreateAll(sampleId, models, variants);
        audit.Entries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.EntriesCreated += variantEntries.Count();
    }
}
