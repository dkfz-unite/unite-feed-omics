using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories;

namespace Unite.Genome.Feed.Data.Writers;

public abstract class DataWriter<TModel, TAudit> : Unite.Data.Context.Services.DataWriter<TModel, TAudit>
    where TAudit : DataWriteAudit, new()
{
    protected ResourceRepository _resourceRepository;

    protected DataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        using var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }

    protected void WriteResources(int sampleId, IEnumerable<ResourceModel> models, ref TAudit audit)
    {
        foreach (var model in models)
        {
            var resource = _resourceRepository.Find(sampleId, model);

            if (resource == null)
            {
                _resourceRepository.Create(sampleId, model);
                audit.ResourcesCreated++;
            }
            else
            {
                _resourceRepository.Update(resource, model);
                audit.ResourcesUpdated++;
            }
       }   
    }
}
