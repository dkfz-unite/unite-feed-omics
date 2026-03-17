using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.Proteins;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class ProteinIndexingContext : IndexingContext<ProteinIndex> 
{
    public List<ProteinExpressionIndex> ProteinExpressionsToAdd { get; } = [];
}

public class ProteinsIndexingHandler: IndexingHandler<ProteinIndex, ProteinsIndexingCache, ProteinIndexEntityBuilder, ProteinIndexingContext>
{
    protected override int BucketSize => _options.BucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.Protein;
    protected override string IndexEntityKind => "Protein";

    private readonly ProteinExpressionIndexEntityBuilder _proteinExpressionIndexEntityBuilder;
    private readonly IIndexService<ProteinExpressionIndex> _proteinExpressionIndexingService;
    private readonly ProteinsIndexingOptions _options;
    
    
    public ProteinsIndexingHandler( 
        IDbContextFactory<DomainDbContext> dbContextFactory,
        TasksProcessingService taskProcessingService,
        ProteinIndexEntityBuilder proteinIndexEntityBuilder,
        IIndexService<ProteinIndex> proteinIndexingService,
        ProteinExpressionIndexEntityBuilder proteinExpressionIndexEntityBuilder,
        IIndexService<ProteinExpressionIndex> proteinExpressionIndexingService,
        ProteinsIndexingOptions options,
        ILogger<ProteinsIndexingHandler> logger) : base(dbContextFactory, taskProcessingService, proteinIndexEntityBuilder, proteinIndexingService, logger)
    {
        _proteinExpressionIndexEntityBuilder = proteinExpressionIndexEntityBuilder;
        _proteinExpressionIndexingService = proteinExpressionIndexingService;
        _options = options;
    }

    protected override async Task BuildIndexEntity(int id, ProteinsIndexingCache indexingCache, ProteinIndexingContext indexingContext)
    {
        await base.BuildIndexEntity(id, indexingCache, indexingContext);
        
        var entities = _proteinExpressionIndexEntityBuilder.Create(id, indexingCache);
        indexingContext.ProteinExpressionsToAdd.AddRange(entities);
    }

    protected override async Task DeleteIndexEntities(ProteinIndexingContext indexingContext)
    {
        await base.DeleteIndexEntities(indexingContext);
        await _proteinExpressionIndexingService.DeleteWhereEquals(index => index.Protein.Id, indexingContext.EntitiesToDelete.Select(id => int.Parse(id)).ToArray());
    }

    protected override async Task CreateIndexEntities(ProteinIndexingContext indexingContext)
    {
        await base.CreateIndexEntities(indexingContext);
        await _proteinExpressionIndexingService.AddRange(indexingContext.ProteinExpressionsToAdd);
    }
}
