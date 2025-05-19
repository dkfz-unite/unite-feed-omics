using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Models.Rna;

namespace Unite.Omics.Annotations.Services.Rna;


public class ExpressionsAnnotationService
{
	private readonly AnnotationsDataLoader _annotationsDataLoader;


    public ExpressionsAnnotationService(IEnsemblDataOptions ensemblOptions, IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _annotationsDataLoader = new AnnotationsDataLoader(ensemblOptions, dbContextFactory);
    }


    public GeneExpressionModel[] AnnotateByGeneId(Dictionary<string, (int Reads, int? Length)> geneExpressions)
    {
        var expressions = _annotationsDataLoader.LoadByGeneId(geneExpressions).Result;

        CalculateNormalizedReads(ref expressions);

        return expressions;
    }

    public GeneExpressionModel[] AnnotateByGeneSymbol(Dictionary<string, (int Reads, int? Length)> geneExpressions)
    {
        var expressions = _annotationsDataLoader.LoadByGeneSymbol(geneExpressions).Result;

        CalculateNormalizedReads(ref expressions);

        return expressions;
    }

    public GeneExpressionModel[] AnnotateByTranscriptId(Dictionary<string, (int Reads, int? Length)> transcriptExpressions)
    {
        var expressions = _annotationsDataLoader.LoadByTranscriptId(transcriptExpressions).Result;

        CalculateNormalizedReads(ref expressions);

        return expressions;
    }

    public GeneExpressionModel[] AnnotateByTranscriptSymbol(Dictionary<string, (int Reads, int? Length)> transcriptExpressions)
    {
        var expressions = _annotationsDataLoader.LoadByTranscriptSymbol(transcriptExpressions).Result;

        CalculateNormalizedReads(ref expressions);

        return expressions;
    }


    private static void CalculateNormalizedReads(ref GeneExpressionModel[] expressions)
	{
		long totalReads = expressions.Sum(e => e.Reads);
		double totalLengt = expressions.Sum(e => e.Reads * 1e3 / e.Gene.ExonicLength.Value);

        foreach (var expression in expressions)
        {
            double tpm = (expression.Reads * 1e3 / expression.Gene.ExonicLength.Value * 1e6) / (totalLengt);
            double fpkm = (expression.Reads * 1e9) / (totalReads * expression.Gene.ExonicLength.Value);

            expression.TPM = Math.Round(tpm, 3);
            expression.FPKM = Math.Round(fpkm, 3);
        }
	}
}
