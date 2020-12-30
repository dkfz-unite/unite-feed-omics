namespace Unite.Mutations.DataFeed.Web.Services
{
    public interface ITaskProcessingService
    {
        void ProcessIndexingTasks(int bucketSize);
    }
}