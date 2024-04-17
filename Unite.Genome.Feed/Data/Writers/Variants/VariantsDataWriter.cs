using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Data.Models;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;

namespace Unite.Genome.Feed.Data.Writers.Variants;

public class VariantsDataWriter : DataWriter<AnalysedSampleModel, VariantsDataUploadAudit>
{
    private Repositories.AnalysisRepository _analysisRepository;
    private Repositories.AnalysedSampleRepository _analysedSampleRepository;
    private Repositories.Variants.SSM.VariantRepository _ssmRepository;
    private Repositories.Variants.SSM.VariantEntryRepository _ssmEntryRepository;
    private Repositories.Variants.CNV.VariantRepository _cnvRepository;
    private Repositories.Variants.CNV.VariantEntryRepository _cnvEntryRepository;
    private Repositories.Variants.SV.VariantRepository _svRepository;
    private Repositories.Variants.SV.VariantEntryRepository _svEntryRepository;


    public VariantsDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _analysisRepository = new Repositories.AnalysisRepository(dbContext);
        _analysedSampleRepository = new Repositories.AnalysedSampleRepository(dbContext);
        _ssmRepository = new Repositories.Variants.SSM.VariantRepository(dbContext);
        _ssmEntryRepository = new Repositories.Variants.SSM.VariantEntryRepository(dbContext, _ssmRepository);
        _cnvRepository = new Repositories.Variants.CNV.VariantRepository(dbContext);
        _cnvEntryRepository = new Repositories.Variants.CNV.VariantEntryRepository(dbContext, _cnvRepository);
        _svRepository = new Repositories.Variants.SV.VariantRepository(dbContext);
        _svEntryRepository = new Repositories.Variants.SV.VariantEntryRepository(dbContext, _svRepository);
    }

    protected override void ProcessModel(AnalysedSampleModel model, ref VariantsDataUploadAudit audit)
    {
        var analysis = _analysisRepository.FindOrCreate(model);

        var analysedSample = _analysedSampleRepository.FindOrCreate(analysis.Id, model);

        if (model.Ssms.IsNotEmpty())
        {
            WriteSsms(analysedSample.Id, model.Ssms, ref audit);
        }

        if (model.Cnvs.IsNotEmpty())
        {
            WriteCnvs(analysedSample.Id, model.Cnvs, ref audit);
        }

        if (model.Svs.IsNotEmpty())
        {
            WriteSvs(analysedSample.Id, model.Svs, ref audit);
        }
    }

    private void WriteSsms(int analysedSampleId, IEnumerable<Models.Variants.SSM.VariantModel> variantModels, ref VariantsDataUploadAudit audit)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var variants = _ssmRepository.CreateMissing(variantModels);
        audit.Ssms.AddRange(variants.Select(variant => variant.Id));
        audit.SsmsCreated += variants.Count();

        var existing = dbContext.Set<Unite.Data.Entities.Genome.Variants.SSM.VariantEntry>().Count(o => o.AnalysedSampleId == analysedSampleId);

        var variantEntries = _ssmEntryRepository.CreateMissing(analysedSampleId, variantModels, variants);
        audit.SsmsEntries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.SsmsAssociated += variantEntries.Count();
        var created = dbContext.Set<Unite.Data.Entities.Genome.Variants.SSM.VariantEntry>().Count(o => o.AnalysedSampleId == analysedSampleId);
    }

    private void WriteCnvs(int analysedSampleId, IEnumerable<Models.Variants.CNV.VariantModel> variantModels, ref VariantsDataUploadAudit audit)
    {
        var variants = _cnvRepository.CreateMissing(variantModels);
        var predicate = GetCnvExcludePredicate();
        audit.Cnvs.AddRange(variants.Where(predicate).Select(variant => variant.Id));
        audit.CnvsCreated += variants.Count();

        var variantEntries = _cnvEntryRepository.CreateMissing(analysedSampleId, variantModels, variants);
        audit.CnvsEntries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.CnvsAssociated += variantEntries.Count();
    }

    private void WriteSvs(int analysedSampleId, IEnumerable<Models.Variants.SV.VariantModel> variantModels, ref VariantsDataUploadAudit audit)
    {
        var variants = _svRepository.CreateMissing(variantModels);
        audit.Svs.AddRange(variants.Select(variant => variant.Id));
        audit.SvsCreated += variants.Count();

        var variantEntries = _svEntryRepository.CreateMissing(analysedSampleId, variantModels, variants);
        audit.SvsEntries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.SvsAssociated += variantEntries.Count();
    }


    /// <summary>
    /// Returns a predicate that excludes CNVs that are neutral and not LOH.
    /// </summary>
    /// <returns>Predicate function.</returns>
    private static Func<CNV.Variant, bool> GetCnvExcludePredicate()
    {
        return variant => !(variant.TypeId == CNV.Enums.CnvType.Neutral && variant.Loh == false);
    }
}
