using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Omics.Feed.Data.Models;
using Unite.Omics.Feed.Data.Repositories;

namespace Unite.Omics.Feed.Data.Writers;

public abstract class DataWriter<TModel, TAudit> : Unite.Data.Context.Services.DataWriter<TModel, TAudit>
    where TAudit : DataWriteAudit, new()
{
    protected SampleRepository _sampleRepository;
    protected ResourceRepository _resourceRepository;


    protected DataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    protected int WriteSample(SampleModel model, ref TAudit audit, SampleModel matchedSample = null)
    {
        var sample = _sampleRepository.Find(model);

        if (sample == null)
        {
            sample = _sampleRepository.Create(model, matchedSample);

            audit.Samples.Add(sample.Id);
            audit.SamplesCreated++;
        }
        else
        {
            _sampleRepository.Update(sample, model, matchedSample);

            audit.Samples.Add(sample.Id);
            audit.SamplesUpdated++;
        }

        return sample.Id;
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
