using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Indices.Services;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler: IndexingHandler<GeneIndex, GenesIndexingCache, GeneIndexEntityBuilder, GeneIndexingContext>
{
    protected override int BucketSize => _options.BucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.Gene;

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

    protected override void BuildIndexEntity(int id, GenesIndexingCache indexingCache, GeneIndexingContext indexingContext)
    {
        base.BuildIndexEntity(id, indexingCache, indexingContext);
        
        var entities = _geneExpressionIndexEntityBuilder.Create(id, indexingCache);

        if (entities != null)
            indexingContext.GeneExpressionsToAdd.AddRange(entities);
    }

    protected override void DeleteIndexEntities(GeneIndexingContext indexingContext)
    {
        base.DeleteIndexEntities(indexingContext);

        if (indexingContext.EntitiesToDelete.Any())
            _geneExpressionIndexingService.DeleteWhereEquals(index => index.Gene.Id, indexingContext.EntitiesToDelete.Select(id => int.Parse(id)).ToArray()).Wait();
    }

    protected override void CreateIndexEntities(GeneIndexingContext indexingContext)
    {
        base.CreateIndexEntities(indexingContext);

        if (indexingContext.GeneExpressionsToAdd.Any())
            _geneExpressionIndexingService.AddRange(indexingContext.GeneExpressionsToAdd).Wait();
    }
}
