namespace Unite.Genome.Feed.Web.Configuration.Options;

public class VariantsAnnotationOptions
{
    /// <summary>
    /// SM annotation bucket size
    /// </summary>
    public int SmBucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_SM_ANNOTATION_BUCKET_SIZE");
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
