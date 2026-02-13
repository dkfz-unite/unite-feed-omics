using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Models.Prot.Readers.DiaNN;

public class Reader : IReader<ExpressionModel>
{
    public string Format => "diann";

    public ExpressionModel[] Read(StreamReader reader)
    {
        try
        {
            var tsv = reader.ReadToEnd();

            var models = TsvReader
                .Read<Expression>(tsv)
                .ToArray();

            // Single accession means reliable protein identification, so it can be used directly.
            var expressions = models
                .Where(model => model.Accessions?.Length == 1)
                .ToDictionary(model => model.Accessions[0], model => model.Intensity);

            // Multiple accessions mena ambiguous protein identification (ordererd by descending probability).
            foreach (var model in models.Where(model => model.Accessions?.Length > 1))
            {
                foreach (var accession in model.Accessions)
                {
                    // Skip accessions already identified with a single accession, as they are more reliable.
                    if (!expressions.ContainsKey(accession))
                        expressions[accession] = model.Intensity;
                }
            }

            return expressions
                .Select(expression => new ExpressionModel{ Accession = expression.Key, Intensity = expression.Value })
                .ToArrayOrNull();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return null;
        }
    }
}
