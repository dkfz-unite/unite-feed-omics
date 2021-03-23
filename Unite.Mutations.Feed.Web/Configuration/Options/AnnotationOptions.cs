namespace Unite.Mutations.Feed.Web.Configuration.Options
{
    public class AnnotationOptions
    {
		/// <summary>
		/// Annotation interval in milliseconds
		/// </summary>
		public int Interval
		{
			get
			{
				var option = EnvironmentConfig.AnnotatioInterval;
				var seconds = int.Parse(option);

				return seconds * 1000;
			}
		}

		/// <summary>
		/// Annotation bucket size
		/// </summary>
		public int BucketSize
		{
			get
			{
				var option = EnvironmentConfig.AnnotationBucketSize;
				var size = int.Parse(option);

				return size;
			}
		}
	}
}
