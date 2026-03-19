using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Models;

namespace Unite.Omics.Annotations.Services.Prot;

public class AnnotationsDataLoader
{
    private readonly GeneDataLoader _geneDataLoader;
    private readonly TranscriptDataLoader _transcriptDataLoader;
    private readonly ProteinDataLoader _proteinDataLoader;


    public AnnotationsDataLoader(IEnsemblDataOptions ensemblOptions, IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _geneDataLoader = new GeneDataLoader(ensemblOptions, dbContextFactory);
        _transcriptDataLoader = new TranscriptDataLoader(ensemblOptions, dbContextFactory);
        _proteinDataLoader = new ProteinDataLoader(ensemblOptions, dbContextFactory);
    }


    public async Task<ProteinModel[]> LoadById(string[] keys)
    {
        var proteins = await _proteinDataLoader.LoadById(keys);
        
        await LoadMissingData(proteins);

        return proteins;
    }

    public async Task<ProteinModel[]> LoadByAccession(string[] keys)
    {
        var proteins = await _proteinDataLoader.LoadByAccession(keys);

        await LoadMissingData(proteins);

        return proteins;
    }

    public async Task<ProteinModel[]> LoadBySymbol(string[] keys)
    {
        var proteins = await _proteinDataLoader.LoadBySymbol(keys);

        await LoadMissingData(proteins);

        return proteins;
    }


    private async Task LoadMissingData(ProteinModel[] proteins)
    {
        var transcriptIds = proteins
            .Select(protein => protein.Transcript.StableId)
            .Distinct()
            .ToArray();

        var transcripts = await _transcriptDataLoader.LoadById(transcriptIds);
        var transcriptsMap = transcripts.ToDictionary(transcript => transcript.StableId);

        var geneIds = transcripts
            .Select(transcript => transcript.Gene.StableId)
            .Distinct()
            .ToArray();

        var genes = await _geneDataLoader.LoadById(geneIds);
        var genesMap = genes.ToDictionary(gene => gene.StableId);

        foreach (var protein in proteins)
        {
            // If this fails, then the data is inconsistent
            protein.Transcript = transcriptsMap[protein.Transcript.StableId];
            protein.Transcript.Gene = genesMap[protein.Transcript.Gene.StableId];
        }
    }
}
