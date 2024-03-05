using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Services.Models.Transcriptomics;

namespace Unite.Genome.Annotations.Services.Transcriptomics;


public class BulkExpressionsAnnotationService
{
	private readonly AnnotationsDataLoader _annotationsDataLoader;


    public BulkExpressionsAnnotationService(IEnsemblDataOptions ensemblOptions, IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _annotationsDataLoader = new AnnotationsDataLoader(ensemblOptions, dbContextFactory);
    }


    public BulkExpressionModel[] AnnotateByGeneId(Dictionary<string, (int Reads, int? Length)> geneExpressions)
    {
        var expressions = _annotationsDataLoader.LoadByGeneId(geneExpressions).Result;

        CalculateNormalizedReads(ref expressions);

        return expressions;
    }

    public BulkExpressionModel[] AnnotateByGeneSymbol(Dictionary<string, (int Reads, int? Length)> geneExpressions)
    {
        var expressions = _annotationsDataLoader.LoadByGeneSymbol(geneExpressions).Result;

        CalculateNormalizedReads(ref expressions);

        return expressions;
    }

    public BulkExpressionModel[] AnnotateByTranscriptId(Dictionary<string, (int Reads, int? Length)> transcriptExpressions)
    {
        var expressions = _annotationsDataLoader.LoadByTranscriptId(transcriptExpressions).Result;

        CalculateNormalizedReads(ref expressions);

        return expressions;
    }

    public BulkExpressionModel[] AnnotateByTranscriptSymbol(Dictionary<string, (int Reads, int? Length)> transcriptExpressions)
    {
        var expressions = _annotationsDataLoader.LoadByTranscriptSymbol(transcriptExpressions).Result;

        CalculateNormalizedReads(ref expressions);

        return expressions;
    }


    private static void CalculateNormalizedReads(ref BulkExpressionModel[] expressions)
	{
		long totalReads = 0;

		double totalNormalizedExonicLength = 0;

		foreach (var expression in expressions)
		{
			totalReads += expression.Reads;

			totalNormalizedExonicLength += expression.Reads * 1e3 / expression.Gene.ExonicLength.Value;
		}

        foreach (var expression in expressions)
		{
			double tpm = ((expression.Reads * 1e3) / (expression.Gene.ExonicLength.Value * 1e6)) / (totalNormalizedExonicLength);

            double fpkm = (expression.Reads * 1e9) / (totalReads * expression.Gene.ExonicLength.Value);

            expression.TPM = Math.Round(tpm, 3);

            expression.FPKM = Math.Round(fpkm, 3);
        }
	}
}
