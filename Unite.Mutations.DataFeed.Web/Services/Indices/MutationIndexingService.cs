using System;
using System.Linq.Expressions;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Mutations.DataFeed.Web.Services.Indices
{
    public class MutationIndexingService : IndexingService<MutationIndex>
    {
        protected override string DefaultIndex
        {
            get { return "mutations"; }
        }

        protected override Expression<Func<MutationIndex, object>> IdProperty
        {
            get { return (mutation) => mutation.Id; }
        }

        public MutationIndexingService(IElasticOptions options) : base(options)
        {

        }
    }
}
