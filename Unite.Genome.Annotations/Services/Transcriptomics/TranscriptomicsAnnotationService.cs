using Unite.Data.Services;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Services.Models.Transcriptomics;

namespace Unite.Genome.Annotations.Services.Transcriptomics;


public class TranscriptomicsAnnotationService
{
	private readonly AnnotationsDataLoader _annotationsDataLoader;


    public TranscriptomicsAnnotationService(IEnsemblOptions ensemblOptions, DomainDbContext dbContext)
    {
        _annotationsDataLoader = new AnnotationsDataLoader(ensemblOptions, dbContext);
    }


    public GeneExpressionModel[] AnnotateByGeneId(Dictionary<string, (int Reads, int? Length)> expressions)
    {
        var geneExpressions = _annotationsDataLoader.LoadByGeneId(expressions).Result;

        CalculateNormalizedReads(ref geneExpressions);

        return geneExpressions;
    }

    public GeneExpressionModel[] AnnotateByGeneSymbol(Dictionary<string, (int Reads, int? Length)> expressions)
    {
        var geneExpressions = _annotationsDataLoader.LoadByGeneSymbol(expressions).Result;

        CalculateNormalizedReads(ref geneExpressions);

        return geneExpressions;
    }

    public GeneExpressionModel[] AnnotateByTranscriptId(Dictionary<string, (int Reads, int? Length)> expressions)
    {
        var geneExpressions = _annotationsDataLoader.LoadByTranscriptId(expressions).Result;

        CalculateNormalizedReads(ref geneExpressions);

        return geneExpressions;
    }

    public GeneExpressionModel[] AnnotateByTranscriptSymbol(Dictionary<string, (int Reads, int? Length)> expressions)
    {
        var geneExpressions = _annotationsDataLoader.LoadByTranscriptSymbol(expressions).Result;

        CalculateNormalizedReads(ref geneExpressions);

        return geneExpressions;
    }


    private static void CalculateNormalizedReads(ref GeneExpressionModel[] geneExpressions)
	{
		long totalReads = 0;

		double totalNormalizedExonicLength = 0;

		foreach (var geneExpression in geneExpressions)
		{
			totalReads += geneExpression.Reads;

			totalNormalizedExonicLength += geneExpression.Reads * 1e3 / geneExpression.Gene.ExonicLength.Value;
		}

        foreach (var geneExpression in geneExpressions)
		{
			double tpm = (geneExpression.Reads * 1e3 / geneExpression.Gene.ExonicLength.Value * 1e6) / (totalNormalizedExonicLength);

            double fpkm = (geneExpression.Reads * 1e9) / (totalReads * geneExpression.Gene.ExonicLength.Value);

            geneExpression.TPM = Math.Round(tpm, 2);

            geneExpression.FPKM = Math.Round(fpkm, 2);
        }
	}
}
