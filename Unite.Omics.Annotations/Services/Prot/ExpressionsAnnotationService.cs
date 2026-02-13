using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Models;
using Unite.Omics.Annotations.Services.Models.Prot;

namespace Unite.Omics.Annotations.Services.Prot;

public class ExpressionsAnnotationService
{
    private readonly AnnotationsDataLoader _annotationsDataLoader;


    public ExpressionsAnnotationService(IEnsemblDataOptions ensemblOptions, IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _annotationsDataLoader = new AnnotationsDataLoader(ensemblOptions, dbContextFactory);
    }


    public ProteinExpressionModel[] AnnotateByProtreinId(Dictionary<string, double> expressions)
    {
        var proteins = _annotationsDataLoader.LoadById(expressions.Keys.ToArray()).Result;

        return Normalize(proteins, expressions, model => model.StableId);
    }

    public ProteinExpressionModel[] AnnotateByProteinAccession(Dictionary<string, double> expressions)
    {
        var proteins = _annotationsDataLoader.LoadByAccession(expressions.Keys.ToArray()).Result;

        return Normalize(proteins, expressions, model => model.Accession);
    }

    public ProteinExpressionModel[] AnnotateByProteinSymbol(Dictionary<string, double> expressions)
    {
        var proteins = _annotationsDataLoader.LoadBySymbol(expressions.Keys.ToArray()).Result;

        return Normalize(proteins, expressions, model => model.Symbol);
    }


    private static ProteinExpressionModel[] Normalize(ProteinModel[] models, Dictionary<string, double> expressions, Func<ProteinModel, string> keySelector)
    {
        var expressionsRaw = models
            .DistinctBy(model => keySelector(model))
            .ToDictionary(model => keySelector(model), model => expressions[keySelector(model)]);

        var expressionsNormalized = CalculateNormalizedIntensities(expressionsRaw);

        var expressionModels = models.Select(model => CreateFrom(model, expressionsRaw[keySelector(model)], expressionsNormalized[keySelector(model)])).ToArray();

        return expressionModels;
    }

    private static ProteinExpressionModel CreateFrom(ProteinModel model, double intensity, double medianCenteredLog2)
    {
        return new ProteinExpressionModel
        {
            Protein = model,
            Intensity = intensity,
            MedianCenteredLog2 = medianCenteredLog2
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
