using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Repositories;

namespace Unite.Genome.Feed.Data.Writers.Transcriptomics;

public class CellDataWriter : DataWriter<AnalysedSampleModel, CellDataUploadAudit>
{
    private AnalysisRepository _analysisRepository;
    private AnalysedSampleRepository _analysedSampleRepository;
    private ResourceRepository _resourceRepository;

    
    public CellDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();
        
        Initialize(dbContext);
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        _analysisRepository = new AnalysisRepository(dbContext);
        _analysedSampleRepository = new AnalysedSampleRepository(dbContext);
        _resourceRepository = new ResourceRepository(dbContext);
    }

    protected override void ProcessModel(AnalysedSampleModel model, ref CellDataUploadAudit audit)
    {
        var analysis = _analysisRepository.FindOrCreate(model);

        var analysedSample = _analysedSampleRepository.FindOrCreate(analysis.Id, model);

        audit.Samples.Add(analysedSample.Id);

        if (model.Resources != null)
        {
            WriteResources(analysedSample.Id, model.Resources, ref audit);
        }
    }


    private void WriteResources(int analysedSampleId, IEnumerable<ResourceModel> models, ref CellDataUploadAudit audit)
    {
        _resourceRepository.RemoveAll(analysedSampleId);

        var resources = _resourceRepository.CreateAll(analysedSampleId, models);

        audit.ResourcesCreated += resources.Count();
    }
}
