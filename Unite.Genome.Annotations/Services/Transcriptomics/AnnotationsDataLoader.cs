using Unite.Data.Entities.Genome;
using Unite.Data.Services;
using Unite.Genome.Annotations.Clients.Ensembl;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Resources;
using Unite.Genome.Annotations.Services.Models;
using Unite.Genome.Annotations.Services.Models.Transcriptomics;

namespace Unite.Genome.Annotations.Services.Transcriptomics;

public class AnnotationsDataLoader
{
    private readonly DomainDbContext _dbContext;
    private readonly EnsemblApiClient1 _ensemblApiClient;


    public AnnotationsDataLoader(IEnsemblOptions ensemblOptions, DomainDbContext dbContext)
    {
        _ensemblApiClient = new EnsemblApiClient1(ensemblOptions);
        _dbContext = dbContext;
    }


    public async Task<GeneExpressionModel[]> LoadByGeneId(Dictionary<string, (int Reads, int? Length)> geneExpressions)
    {
        var genes = await LoadGenesById(geneExpressions.Keys.ToArray());

        return genes.Select(gene => Convert(gene, geneExpressions[gene.Id])).ToArray();
    }

    public async Task<GeneExpressionModel[]> LoadByGeneSymbol(Dictionary<string, (int Reads, int? Length)> geneExpressions)
    {
        var genes = await LoadGenesByName(geneExpressions.Keys.ToArray());

        return genes.Select(gene => Convert(gene, geneExpressions[gene.Symbol])).ToArray();
    }

    public async Task<GeneExpressionModel[]> LoadByTranscriptId(Dictionary<string, (int Reads, int? Length)> transcriptExpressions)
    {
        var transcripts = await LoadTranscriptsById(transcriptExpressions.Keys.ToArray());

        var geneExpressions = transcripts.Select(transcript =>
        {
            var expression = transcriptExpressions[transcript.Id];

            return new KeyValuePair<string, (int Reads, int? Length)>(transcript.GeneId, (expression.Reads, expression.Length));

        }).ToDictionary(item => item.Key, item => item.Value);

        var genes = await LoadGenesById(geneExpressions.Keys.ToArray());

        return genes.Select(gene => Convert(gene, geneExpressions[gene.Id])).ToArray();
    }

    public async Task<GeneExpressionModel[]> LoadByTranscriptSymbol(Dictionary<string, (int Reads, int? Length)> transcriptExpressions)
    {
        var transcripts = await LoadTranscriptsByName(transcriptExpressions.Keys.ToArray());

        var geneExpressions = transcripts.Select(transcript =>
        {
            var expression = transcriptExpressions[transcript.Symbol];

            return new KeyValuePair<string, (int Reads, int? Length)>(transcript.GeneId, (expression.Reads, expression.Length));

        }).ToDictionary(item => item.Key, item => item.Value);

        var genes = await LoadGenesById(geneExpressions.Keys.ToArray());

        return genes.Select(gene => Convert(gene, geneExpressions[gene.Id])).ToArray();
    }


    private async Task<GeneResource[]> LoadGenesById(string[] identifiers)
    {
        var existingGenes = _dbContext.Set<Gene>().Where(gene => identifiers.Contains(gene.StableId)).Select(Convert).ToArray();

        var existingIdentifiers = existingGenes.Select(gene => gene.Id).ToArray();

        var newIdentifiers = identifiers.Except(existingIdentifiers).ToArray();

        var newGenes = await _ensemblApiClient.Find<GeneResource>(newIdentifiers, length: true, expand: false);

        return Enumerable.Concat(existingGenes, newGenes).ToArray();
    }

    private async Task<GeneResource[]> LoadGenesByName(string[] symbols)
    {
        var existingGenes = _dbContext.Set<Gene>().Where(gene => symbols.Contains(gene.Symbol)).Select(Convert).ToArray();

        var existingSymbols = existingGenes.Select(gene => gene.Symbol).ToArray();

        var newSymbols = symbols.Except(existingSymbols).ToArray();

        var newGenes = await _ensemblApiClient.FindByName<GeneResource>(newSymbols, length: true, expand: false);

        return Enumerable.Concat(existingGenes, newGenes).ToArray();
    }

    private async Task<TranscriptResource[]> LoadTranscriptsById(string[] identifiers)
    {
        var existingTranscripts = _dbContext.Set<Transcript>().Where(transcript => identifiers.Contains(transcript.StableId)).Select(Convert).ToArray();

        var existingIdentifiers = existingTranscripts.Select(transcript => transcript.Id).ToArray();

        var newIdentifiers = identifiers.Except(existingIdentifiers).ToArray();

        var newTranscripts = await _ensemblApiClient.Find<TranscriptResource>(newIdentifiers, length: true, expand: true);

        return Enumerable.Concat(existingTranscripts, newTranscripts).ToArray();
    }

    private async Task<TranscriptResource[]> LoadTranscriptsByName(string[] symbols)
    {
        var existingTranscripts = _dbContext.Set<Transcript>().Where(transcript => symbols.Contains(transcript.Symbol)).Select(Convert).ToArray();

        var existingSymbols = existingTranscripts.Select(transcript => transcript.Symbol).ToArray();

        var newSymbols = symbols.Except(existingSymbols).ToArray();

        var newTranscripts = await _ensemblApiClient.FindByName<TranscriptResource>(newSymbols, length: true, expand: true);

        return Enumerable.Concat(existingTranscripts, newTranscripts).ToArray();
    }


    private static (string Identifier, int? Length, int Reads) FindExpression(string Identifier, (string Identifier, int? Length, int Reads)[] expressions)
    {
        return expressions.First(expression => expression.Identifier == Identifier);
    }

    private static GeneExpressionModel Convert(GeneResource resource, (int Reads, int? Length) expression)
    {
        return new GeneExpressionModel
        {
            Gene = Convert(resource, expression.Length),
            Reads = expression.Reads
        };
    }

    private static GeneModel Convert(GeneResource resource, int? length)
    {
        return new GeneModel
        {
            Id = resource.Id,
            Symbol = resource.Symbol,
            Description = resource.Description,
            Chromosome = resource.Chromosome,
            Start = resource.Start,
            End = resource.End,
            Strand = resource.Strand,
            Biotype = resource.Biotype,
            ExonicLength = length ?? resource.ExonicLength
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
}
