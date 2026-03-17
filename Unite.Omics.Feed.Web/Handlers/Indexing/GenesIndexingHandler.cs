using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Indices.Services;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GeneIndexingContext : IndexingContext<GeneIndex> 
{
    public List<GeneExpressionIndex> GeneExpressionsToAdd { get; } = [];
}

public class GenesIndexingHandler: IndexingHandler<GeneIndex, GenesIndexingCache, GeneIndexEntityBuilder, GeneIndexingContext>
{
    private readonly GenesIndexingOptions _options;
    private readonly IIndexService<GeneExpressionIndex> _expressionsIndexingService;
    private readonly GeneExpressionIndexEntityBuilder _geneExpressionIndexEntityBuilder;
    
    public GenesIndexingHandler(TasksProcessingService taskProcessingService, 
        IDbContextFactory<DomainDbContext> dbContextFactory, 
        ILogger<GenesIndexingHandler> logger, 
        IIndexService<GeneIndex> indexingService, 
        GeneIndexEntityBuilder indexEntityBuilder, 
        IIndexService<GeneExpressionIndex> expressionsIndexingService, 
        GeneExpressionIndexEntityBuilder geneExpressionIndexEntityBuilder, 
        GenesIndexingOptions options) : base(taskProcessingService, dbContextFactory, logger, indexingService, indexEntityBuilder)
    {
        _expressionsIndexingService = expressionsIndexingService;
        _geneExpressionIndexEntityBuilder = geneExpressionIndexEntityBuilder;
        _options = options;
    }

    protected override int BucketSize => _options.BucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.Gene;
    protected override string IndexEntityKind => "Gene";

    protected override async Task BuildIndexEntity(int id, GenesIndexingCache indexingCache, GeneIndexingContext indexingContext)
    {
        await base.BuildIndexEntity(id, indexingCache, indexingContext);
        
        var entities = _geneExpressionIndexEntityBuilder.Create(id, indexingCache);
        indexingContext.GeneExpressionsToAdd.AddRange(entities);
    }

    protected override async Task DeleteIndexEntities(GeneIndexingContext indexingContext)
    {
        await base.DeleteIndexEntities(indexingContext);
        await _expressionsIndexingService.DeleteWhereEquals(index => index.Gene.Id, indexingContext.EntitiesToDelete.Select(id => int.Parse(id)).ToArray());
    }

    protected override async Task CreateIndexEntities(GeneIndexingContext indexingContext)
    {
        await base.CreateIndexEntities(indexingContext);
        await _expressionsIndexingService.AddRange(indexingContext.GeneExpressionsToAdd);
    }
}