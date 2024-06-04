using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories.Constants;
using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Data.Writers.Dna;

public class AnalysisWriter : DataWriter<SampleModel, AnalysisWriteAudit>
{
    private Repositories.SampleRepository _sampleRepository;
    private Repositories.Dna.Ssm.VariantRepository _ssmRepository;
    private Repositories.Dna.Ssm.VariantEntryRepository _ssmEntryRepository;
    private Repositories.Dna.Cnv.VariantRepository _cnvRepository;
    private Repositories.Dna.Cnv.VariantEntryRepository _cnvEntryRepository;
    private Repositories.Dna.Sv.VariantRepository _svRepository;
    private Repositories.Dna.Sv.VariantEntryRepository _svEntryRepository;


    public AnalysisWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _sampleRepository = new Repositories.SampleRepository(dbContext);
        _ssmRepository = new Repositories.Dna.Ssm.VariantRepository(dbContext);
        _ssmEntryRepository = new Repositories.Dna.Ssm.VariantEntryRepository(dbContext, _ssmRepository);
        _cnvRepository = new Repositories.Dna.Cnv.VariantRepository(dbContext);
        _cnvEntryRepository = new Repositories.Dna.Cnv.VariantEntryRepository(dbContext, _cnvRepository);
        _svRepository = new Repositories.Dna.Sv.VariantRepository(dbContext);
        _svEntryRepository = new Repositories.Dna.Sv.VariantEntryRepository(dbContext, _svRepository);
        _resourceRepository = new Repositories.ResourceRepository(dbContext);
    }

    protected override void ProcessModel(SampleModel model, ref AnalysisWriteAudit audit)
    {
        var sample = _sampleRepository.FindOrCreate(model);

        if (model.Ssms.IsNotEmpty())
            WriteSsms(sample.Id, model.Ssms, ref audit);

        if (model.Cnvs.IsNotEmpty())
            WriteCnvs(sample.Id, model.Cnvs, ref audit);

        if (model.Svs.IsNotEmpty())
            WriteSvs(sample.Id, model.Svs, ref audit);

        if (model.Resources.IsNotEmpty())
            WriteResources(sample.Id, model.Resources, ref audit);
    }

    private void WriteSsms(int sampleId, IEnumerable<Models.Dna.Ssm.VariantModel> models, ref AnalysisWriteAudit audit)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var variants = _ssmRepository.CreateMissing(models);
        audit.Ssms.AddRange(variants.Select(variant => variant.Id));
        audit.SsmsCreated += variants.Count();

        var variantEntries = _ssmEntryRepository.CreateMissing(sampleId, models, variants);
        audit.SsmsEntries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.SsmsAssociated += variantEntries.Count();
    }

    private void WriteCnvs(int sampleId, IEnumerable<Models.Dna.Cnv.VariantModel> models, ref AnalysisWriteAudit audit)
    {
        var variants = _cnvRepository.CreateMissing(models);
        var predicate = Predicates.IsInfluentCnv.Compile();
        audit.Cnvs.AddRange(variants.Where(predicate).Select(variant => variant.Id));
        audit.CnvsCreated += variants.Count();

        var variantEntries = _cnvEntryRepository.CreateMissing(sampleId, models, variants);
        audit.CnvsEntries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.CnvsAssociated += variantEntries.Count();
    }

    private void WriteSvs(int sampleId, IEnumerable<Models.Dna.Sv.VariantModel> models, ref AnalysisWriteAudit audit)
    {
        var variants = _svRepository.CreateMissing(models);
        audit.Svs.AddRange(variants.Select(variant => variant.Id));
        audit.SvsCreated += variants.Count();

        var variantEntries = _svEntryRepository.CreateMissing(sampleId, models, variants);
        audit.SvsEntries.AddRange(variantEntries.Select(entry => entry.EntityId));
        audit.SvsAssociated += variantEntries.Count();
    }
}
