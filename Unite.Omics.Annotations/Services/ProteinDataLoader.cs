using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Omics.Annotations.Clients.Ensembl;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources;
using Unite.Omics.Annotations.Services.Models;

namespace Unite.Omics.Annotations.Services;

public class ProteinDataLoader
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly EnsemblApiClient1 _ensemblApiClient;


    public ProteinDataLoader(IEnsemblDataOptions ensemblOptions, IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _ensemblApiClient = new EnsemblApiClient1(ensemblOptions);
    }


    public async Task<ProteinModel[]> LoadById(string[] keys)
    {
        var existingEntities = LoadEntities(entity => keys.Contains(entity.StableId));

        var existingKeys = existingEntities.Select(entity => entity.StableId).ToArray();

        var newKeys = keys.Except(existingKeys).ToArray();

        var newResources = await _ensemblApiClient.FindById<ProteinResource>(newKeys, length: true, expand: false);

        return Enumerable.Concat(existingEntities.Select(Convert), newResources.Select(Convert)).ToArray();
    }

    public async Task<ProteinModel[]> LoadByAccession(string[] keys)
    {
        var existingEntities = LoadEntities(entity => keys.Contains(entity.AccessionId));

        var existingKeys = existingEntities.Select(entity => entity.AccessionId).ToArray();

        var newKeys = keys.Except(existingKeys).ToArray();

        var newResources = await _ensemblApiClient.FindByAccession(newKeys, length: true, expand: false);

        return Enumerable.Concat(existingEntities.Select(Convert), newResources.Select(Convert)).ToArray();
    }

    public async Task<ProteinModel[]> LoadBySymbol(string[] keys)
    {
        var existingEntities = LoadEntities(entity => keys.Contains(entity.Symbol));

        var existingKeys = existingEntities.Select(entity => entity.Symbol).ToArray();

        var newKeys = keys.Except(existingKeys).ToArray();

        var newResources = await _ensemblApiClient.FindByName<ProteinResource>(newKeys, length: true, expand: false);

        return Enumerable.Concat(existingEntities.Select(Convert), newResources.Select(Convert)).ToArray();
    }


    private Protein[] LoadEntities(Expression<Func<Protein, bool>> predicate)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Protein>()
            .AsNoTracking()
            .Include(entity => entity.Transcript)
            .Where(predicate)
            .ToArray();
    }

    private static ProteinModel Convert(Protein entity)
    {
        return new ProteinModel
        {
            Id = entity.Id,
            StableId = entity.StableId,
            // Accession = entity.AccessionId,
            // Symbol = entity.Symbol,
            // Description = entity.Description,
            // Database = entity.Database,
            // Start = entity.Start.Value,
            // End = entity.End.Value,
            // Length = entity.Length.Value,
            // IsCanonical = entity.IsCanonical.Value,
            
            Transcript = new () { Id = entity.Transcript.Id, StableId = entity.Transcript.StableId }
        };
    }

    private static ProteinModel Convert(ProteinResource resource)
    {
        return new ProteinModel
        {
            Id = null,
            StableId = resource.Id,
            Accession = resource.Accession,
            Symbol = resource.Symbol,
            Description = resource.Description,
            Database = resource.Database,
            Start = resource.Start,
            End = resource.End,
            Length = resource.Length,
            IsCanonical = resource.IsCanonical,

            Transcript = new () { StableId = resource.TranscriptId }
        };
    }
}
