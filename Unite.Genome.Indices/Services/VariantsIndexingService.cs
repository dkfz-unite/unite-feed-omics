using System.Linq.Expressions;
using Unite.Indices.Entities.Variants;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Genome.Indices.Services;

public class VariantsIndexingService : IndexingService<VariantIndex>
{
    protected override string DefaultIndex
    {
        get { return "variants"; }
    }

    protected override Expression<Func<VariantIndex, object>> IdProperty
    {
        get { return (mutation) => mutation.Id; }
    }


    public VariantsIndexingService(IElasticOptions options) : base(options)
    {
    }
}
