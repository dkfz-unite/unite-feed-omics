using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Omics.Annotations.Clients.Ensembl;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources;
using Unite.Omics.Annotations.Services.Models;

namespace Unite.Omics.Annotations.Services;

public class GeneDataLoader
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly EnsemblApiClient1 _ensemblApiClient;


    public GeneDataLoader(IEnsemblDataOptions ensemblOptions, IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _ensemblApiClient = new EnsemblApiClient1(ensemblOptions);
    }


    public async Task<GeneModel[]> LoadById(string[] keys)
    {
        var existingEntities = LoadEntities(entity => keys.Contains(entity.StableId));

        var existingKeys = existingEntities.Select(entity => entity.StableId).ToArray();

        var newKeys = keys.Except(existingKeys).ToArray();

        var newResources = await _ensemblApiClient.FindById<GeneResource>(newKeys, length: true, expand: false);

        return Enumerable.Concat(existingEntities.Select(Convert), newResources.Select(Convert)).ToArray();
    }

    public async Task<GeneModel[]> LoadBySymbol(string[] keys)
    {
        var existingEntities = LoadEntities(entity => keys.Contains(entity.Symbol));

        var existingKeys = existingEntities.Select(entity => entity.Symbol).ToArray();

        var newKeys = keys.Except(existingKeys).ToArray();

        var newResources = await _ensemblApiClient.FindByName<GeneResource>(newKeys, length: true, expand: false);

        return Enumerable.Concat(existingEntities.Select(Convert), newResources.Select(Convert)).ToArray();
    }


    private Gene[] LoadEntities(Expression<Func<Gene, bool>> predicate)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Gene>()
            .AsNoTracking()
            .Where(predicate)
            .ToArray();
    }

    private static GeneModel Convert(Gene entity)
    {
        return new GeneModel
        {
            Id = entity.Id,
            StableId = entity.StableId,
            Symbol = entity.Symbol,
            Description = entity.Description,
            Biotype = entity.Biotype,
            Chromosome = entity.ChromosomeId.Value,
            Start = entity.Start.Value,
            End = entity.End.Value,
            Strand = entity.Strand.Value,
            ExonicLength = entity.ExonicLength
        };
    }

    private static GeneModel Convert(GeneResource resource)
    {
        return new GeneModel
        {
            Id = null,
            StableId = resource.Id,
            Symbol = resource.Symbol,
            Description = resource.Description,
            Biotype = resource.Biotype,
            Chromosome = resource.Chromosome,
            Start = resource.Start,
            End = resource.End,
            Strand = resource.Strand,
            ExonicLength = resource.ExonicLength
        };
    }
}
