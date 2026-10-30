using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Omics.Feed.Data.Models;
using Unite.Omics.Feed.Data.Repositories;

namespace Unite.Omics.Feed.Data.Writers;

public abstract class AnalysisWriter<TModel, TAudit> : DataWriter<TModel, TAudit>
    where TModel : AnalysisModel
    where TAudit : DataWriteAudit, new()
{
    protected AnalysisWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _sampleRepository = new SampleRepository(dbContext);
        _resourceRepository = new ResourceRepository(dbContext);
        InitializeEntries(dbContext);
    }

    protected override void ProcessModel(TModel model, ref TAudit audit)
    {
        var sampleId = WriteSample(model.TargetSample, ref audit, model.MatchedSample);
        WriteEntries(sampleId, model, ref audit);
        WriteResources(sampleId, model.TargetSample.Resources ?? [], ref audit);
        WriteResources(sampleId, model.Resources ?? [], ref audit);
    }

    protected virtual void InitializeEntries(DomainDbContext dbContext)
    {
    }

    protected virtual void WriteEntries(int sampleId, TModel model, ref TAudit audit)
    {
    }
}
