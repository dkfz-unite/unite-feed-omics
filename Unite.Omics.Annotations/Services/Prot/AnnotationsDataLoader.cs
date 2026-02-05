using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Omics.Annotations.Clients.Ensembl;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources;
using Unite.Omics.Annotations.Services.Models;
using Unite.Omics.Annotations.Services.Models.Prot;

namespace Unite.Omics.Annotations.Services.Prot;

public class AnnotationsDataLoader
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly EnsemblApiClient1 _ensemblApiClient;


    public AnnotationsDataLoader(IEnsemblDataOptions ensemblOptions, IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _ensemblApiClient = new EnsemblApiClient1(ensemblOptions);
        _dbContextFactory = dbContextFactory;
    }


    // public async Task<ProteinModel[]> LoadByProteinId(string[] proteinIds)
    // {
    //     var proteins = await LoadProteinsById(proteinIds);

    //     return proteins.Select(protein => Convert(protein)).ToArray();
    // }


    private async Task<ProteinResource[]> LoadProteinsById(string[] identifiers)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var existingProteins = dbContext.Set<Protein>().AsNoTracking().Where(protein => identifiers.Contains(protein.StableId)).Select(Convert).ToArray();

        var existingIdentifiers = existingProteins.Select(protein => protein.Id).ToArray();

        // TODO: Wrong IDs are compared
        var newIdentifiers = identifiers.Except(existingIdentifiers).ToArray();

        var newProteins = await _ensemblApiClient.Find<ProteinResource>(newIdentifiers, length: true, expand: false);

        return Enumerable.Concat(existingProteins, newProteins).ToArray();
    }

    private async Task<ProteinResource[]> LoadProteinsByAccession(string[] identifiers)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var existingProteins = dbContext.Set<Protein>().AsNoTracking().Where(protein => identifiers.Contains(protein.AccessionId)).Select(Convert).ToArray();

        var existingIdentifiers = existingProteins.Select(protein => protein.Id).ToArray();

        var newIdentifiers = identifiers.Except(existingIdentifiers).ToArray();

        var newProteins = await _ensemblApiClient.Find<ProteinResource>(newIdentifiers, length: true, expand: false);

        return Enumerable.Concat(existingProteins, newProteins).ToArray();
    }


    private static ProteinExpressionModel Convert(ProteinResource resource, double expression)
    {
        return new ProteinExpressionModel
        {
            Protein = Convert(resource),
            Intensity = expression
        };
    }

    private static ProteinModel Convert(ProteinResource resource)
    {
        return new ProteinModel
        {
            Id = resource.Id,
            Accession = resource.Accession,
            Symbol = resource.Symbol,
            Description = resource.Description,
            Database = resource.Database,
            Start = resource.Start,
            End = resource.End,
            Length = resource.Length,
            IsCanonical = resource.IsCanonical
        };
    }

    private static GeneResource Convert(Gene entity)
    {
        return new GeneResource
        {
            Id = entity.StableId,
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

    private static TranscriptResource Convert(Transcript entity)
    {
        return new TranscriptResource
        {
            Id = entity.StableId,
            Symbol = entity.Symbol,
            Description = entity.Description,
            Biotype = entity.Biotype,
            IsCanonical = entity.IsCanonical.Value,
            Chromosome = entity.ChromosomeId.Value,
            Start = entity.Start.Value,
            End = entity.End.Value,
            Strand = entity.Strand.Value,
            ExonicLength = entity.ExonicLength
        };
    }

    private static ProteinResource Convert(Protein entity)
    {
        return new ProteinResource
        {
            Id = entity.StableId,
            Accession = entity.AccessionId,
            Symbol = entity.Symbol,
            Description = entity.Description,
            Database = entity.Database,
            Start = entity.Start.Value,
            End = entity.End.Value,
            Length = entity.Length.Value,
            IsCanonical = entity.IsCanonical.Value
        };
    }
}
