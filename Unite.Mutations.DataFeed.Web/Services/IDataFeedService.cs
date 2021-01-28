using System.Collections.Generic;

namespace Unite.Mutations.DataFeed.Web.Services
{
    public interface IDataFeedService<T> where T : class
    {
        void ProcessResources(IEnumerable<T> resources);
    }
}