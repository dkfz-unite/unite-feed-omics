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
    protected override int BucketSize => _options.BucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.Gene;
    protected override string IndexEntityKind => "Gene";

    private readonly GeneExpressionIndexEntityBuilder _geneExpressionIndexEntityBuilder;
    private readonly IIndexService<GeneExpressionIndex> _geneExpressionIndexingService;
    private readonly GenesIndexingOptions _options;
    
    
    public GenesIndexingHandler( 
        IDbContextFactory<DomainDbContext> dbContextFactory,
        TasksProcessingService taskProcessingService,
        GeneIndexEntityBuilder geneIndexEntityBuilder,
        IIndexService<GeneIndex> geneIndexingService,
        GeneExpressionIndexEntityBuilder geneExpressionIndexEntityBuilder,
        IIndexService<GeneExpressionIndex> geneExpressionIndexingService,
        GenesIndexingOptions options,
        ILogger<GenesIndexingHandler> logger
        ) : base(dbContextFactory, taskProcessingService, geneIndexEntityBuilder, geneIndexingService, logger)
    {
        _geneExpressionIndexEntityBuilder = geneExpressionIndexEntityBuilder;
        _geneExpressionIndexingService = geneExpressionIndexingService;
        _options = options;
    }

    protected override async Task BuildIndexEntity(int id, GenesIndexingCache indexingCache, GeneIndexingContext indexingContext)
    {
        await base.BuildIndexEntity(id, indexingCache, indexingContext);
        
        var entities = _geneExpressionIndexEntityBuilder.Create(id, indexingCache);
        indexingContext.GeneExpressionsToAdd.AddRange(entities);
    }

    protected override async Task DeleteIndexEntities(GeneIndexingContext indexingContext)
    {
        await base.DeleteIndexEntities(indexingContext);

        if (indexingContext.EntitiesToDelete.Any())
            await _geneExpressionIndexingService.DeleteWhereEquals(index => index.Gene.Id, indexingContext.EntitiesToDelete.Select(id => int.Parse(id)).ToArray());
    }

    protected override async Task CreateIndexEntities(GeneIndexingContext indexingContext)
    {
        await base.CreateIndexEntities(indexingContext);

        if (indexingContext.GeneExpressionsToAdd.Any())
            await _geneExpressionIndexingService.AddRange(indexingContext.GeneExpressionsToAdd);
    }
}
