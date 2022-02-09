using System;
using System.Linq.Expressions;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Genome.Indices.Services
{
    public class MutationsIndexingService : IndexingService<MutationIndex>
    {
        protected override string DefaultIndex
        {
            get { return "mutations"; }
        }

        protected override Expression<Func<MutationIndex, object>> IdProperty
        {
            get { return (mutation) => mutation.Id; }
        }


        public MutationsIndexingService(IElasticOptions options) : base(options)
        {

        }
    }
}
