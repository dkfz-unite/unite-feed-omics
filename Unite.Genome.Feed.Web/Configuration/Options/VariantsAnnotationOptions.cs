namespace Unite.Genome.Feed.Web.Configuration.Options;

public class VariantsAnnotationOptions
{
    /// <summary>
    /// SSM annotation bucket size
    /// </summary>
    public int SsmBucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_SSM_ANNOTATION_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }

    /// <summary>
    /// CNV annotation bucket size
    /// </summary>
    public int CnvBucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_CNV_ANNOTATION_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }

    /// <summary>
    /// SV annotation bucket size
    /// </summary>
    public int SvBucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_SV_ANNOTATION_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }
}
