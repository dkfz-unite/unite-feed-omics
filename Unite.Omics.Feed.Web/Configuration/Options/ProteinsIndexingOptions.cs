namespace Unite.Omics.Feed.Web.Configuration.Options;

public class ProteinsIndexingOptions
{
    /// <summary>
    /// Indexing bucket size
    /// </summary>
    public int BucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_PROTEINS_INDEXING_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }
}
