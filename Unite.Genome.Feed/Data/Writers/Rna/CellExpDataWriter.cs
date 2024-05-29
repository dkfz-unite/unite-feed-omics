using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories;

namespace Unite.Genome.Feed.Data.Writers.Rna;

public class CellExpDataWriter : DataWriter<SampleModel, CellExpDataWriteAudit>
{
    private SampleRepository _sampleRepository;

    public CellExpDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        _sampleRepository = new SampleRepository(dbContext);
        _resourceRepository = new ResourceRepository(dbContext);
    }

    protected override void ProcessModel(SampleModel model, ref CellExpDataWriteAudit audit)
    {
        var analysedSample = _sampleRepository.FindOrCreate(model);

        audit.Samples.Add(analysedSample.Id);

        if (model.Resources != null)
        {
            WriteResources(analysedSample.Id, model.Resources, ref audit);
        }
    }
}
