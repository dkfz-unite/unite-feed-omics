namespace Unite.Omics.Feed.Web.Configuration.Options;

public class VariantsIndexingOptions
{
    /// <summary>
    /// SM indexing bucket size
    /// </summary>
    public int SmBucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_SM_INDEXING_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }

    /// <summary>
    /// CNV indexing bucket size
    /// </summary>
    public int CnvBucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_CNV_INDEXING_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }

    /// <summary>
    /// SV indexing bucket size
    /// </summary>
    public int SvBucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_SV_INDEXING_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }
}
