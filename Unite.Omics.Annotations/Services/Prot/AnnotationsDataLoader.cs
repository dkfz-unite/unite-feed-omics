using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Omics.Annotations.Clients.Ensembl;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources;

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


    // private async Task<ProteinResource[]> LoadGenesById(string[] identifiers)
    // {
    //     using var dbContext = _dbContextFactory.CreateDbContext();

    //     var existingProteins = dbContext.Set<Protein>().AsNoTracking().Where(protein => identifiers.Contains(protein.StableId)).Select(Convert).ToArray();

    //     var existingIdentifiers = existingProteins.Select(protein => protein.Id).ToArray();

    //     var newIdentifiers = identifiers.Except(existingIdentifiers).ToArray();

    //     var newProteins = await _ensemblApiClient.Find<GeneResource>(newIdentifiers, length: true, expand: false);

    //     return Enumerable.Concat(existingProteins, newProteins).ToArray();
    // }
}
