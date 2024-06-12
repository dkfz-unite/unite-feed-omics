using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories;

namespace Unite.Genome.Feed.Data.Writers.RnaSc;

public class AnalysisWriter : DataWriter<SampleModel, AnalysisWriteAudit>
{
    private SampleRepository _sampleRepository;

    public AnalysisWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        _sampleRepository = new SampleRepository(dbContext);
        _resourceRepository = new ResourceRepository(dbContext);
    }

    protected override void ProcessModel(SampleModel model, ref AnalysisWriteAudit audit)
    {
        var sample = _sampleRepository.FindOrCreate(model);

        audit.Samples.Add(sample.Id);

        if (model.Resources != null)
        {
            WriteResources(sample.Id, model.Resources, ref audit);
        }
    }
}
