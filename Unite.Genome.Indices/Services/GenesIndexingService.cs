using System;
using System.Linq.Expressions;
using Unite.Indices.Entities.Genes;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Genome.Indices.Services
{
    public class GenesIndexingService : IndexingService<GeneIndex>
    {
        protected override string DefaultIndex
        {
            get { return "genes"; }
        }

        protected override Expression<Func<GeneIndex, object>> IdProperty
        {
            get { return (gene) => gene.Id; }
        }


        public GenesIndexingService(IElasticOptions options) : base(options)
        {
        }
    }
}
