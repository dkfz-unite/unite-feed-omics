using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Models;
using Unite.Omics.Annotations.Models.Prot;

namespace Unite.Omics.Annotations.Services.Prot;

public class ExpressionsAnnotationService
{
    private readonly AnnotationsDataLoader _annotationsDataLoader;


    public ExpressionsAnnotationService(IEnsemblDataOptions ensemblOptions, IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _annotationsDataLoader = new AnnotationsDataLoader(ensemblOptions, dbContextFactory);
    }


    public ProteinExpressionModel[] AnnotateByProtreinId((string Key, double Value, string[] Group)[] expressions)
    {
        var proteins = _annotationsDataLoader.LoadById(expressions.Select(expression => expression.Key).ToArray()).Result;

        return Normalize(proteins, expressions, model => model.StableId);
    }

    public ProteinExpressionModel[] AnnotateByProteinAccession((string Key, double Value, string[] Group)[] expressions)
    {
        var proteins = _annotationsDataLoader.LoadByAccession(expressions.Select(expression => expression.Key).ToArray()).Result;

        return Normalize(proteins, expressions, model => model.Accession);
    }

    public ProteinExpressionModel[] AnnotateByProteinSymbol((string Key, double Value, string[] Group)[] expressions)
    {
        var proteins = _annotationsDataLoader.LoadBySymbol(expressions.Select(expression => expression.Key).ToArray()).Result;

        return Normalize(proteins, expressions, model => model.Symbol);
    }


    private static ProteinExpressionModel[] Normalize(ProteinModel[] models, (string Key, double Value, string[] Group)[] expressions, Func<ProteinModel, string> keySelector)
    {
        var proteins = models.ToLookup(keySelector);

        var selectedExpressions = expressions
            .SelectMany(expression => proteins[expression.Key]
                .Select(protein => (Protein: protein, expression.Value, GroupSize: expression.Group?.Length ?? 1)))
            .GroupBy(expression => expression.Protein.StableId)
            .Select(group => group
                .OrderBy(expression => expression.GroupSize)
                .First())
            .ToArray();

        var expressionsRaw = selectedExpressions
            .ToDictionary(expression => expression.Protein.StableId, expression => expression.Value);

        var expressionsNormalized = CalculateNormalizedIntensities(expressionsRaw);

        var expressionModels = selectedExpressions
            .Select(expression => CreateFrom(
                expression.Protein,
                expression.Value,
                expressionsNormalized[expression.Protein.StableId]))
            .ToArray();

        return expressionModels;
    }

    private static ProteinExpressionModel CreateFrom(ProteinModel model, double intensity, double medianCenteredLog2)
    {
        return new ProteinExpressionModel
        {
            Protein = model,
            Raw = intensity,
            Normalized = medianCenteredLog2
        };
    }

    private static Dictionary<string, double> CalculateNormalizedIntensities(Dictionary<string, double> expressions)
    {
        var log2 = expressions.ToDictionary(e => e.Key, e => Math.Log2(e.Value + 1));
        var log2Median = log2.Values.Order().ElementAt(log2.Count / 2);
        var log2MedianCentered = log2.ToDictionary(e => e.Key, e => e.Value - log2Median);

        return log2MedianCentered;
    }
}
