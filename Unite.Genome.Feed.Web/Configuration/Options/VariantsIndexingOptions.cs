namespace Unite.Genome.Feed.Web.Configuration.Options;

public class VariantsIndexingOptions
{
    /// <summary>
    /// SSM indexing bucket size
    /// </summary>
    public int SsmBucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_SSM_INDEXING_BUCKET_SIZE");
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
