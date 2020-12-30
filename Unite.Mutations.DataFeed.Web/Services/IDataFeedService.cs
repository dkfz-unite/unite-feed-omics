using System.Collections.Generic;
using Unite.Mutations.DataFeed.Domain.Resources.Samples;

namespace Unite.Mutations.DataFeed.Web.Services
{
    public interface IDataFeedService
    {
        void ProcessSamples(IEnumerable<Sample> samples);
    }
}