using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Data.Writers;

public class SampleWriter : DataWriter<SampleModel, SampleWriteAudit>
{
    public SampleWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _sampleRepository = new Repositories.SampleRepository(dbContext);
        _resourceRepository = new Repositories.ResourceRepository(dbContext);
    }

    protected override void ProcessModel(SampleModel model, ref SampleWriteAudit audit)
    {
        var sampleId = WriteSample(model, ref audit);

        if (model.Resources.IsNotEmpty())
            WriteResources(sampleId, model.Resources, ref audit);
    }
}
