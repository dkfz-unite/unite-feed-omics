using System;

namespace Unite.Genome.Feed.Web.Configuration.Options
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
				var option = Environment.GetEnvironmentVariable("UNITE_ANNOTATION_INTERVAL");
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
				var option = Environment.GetEnvironmentVariable("UNITE_ANNOTATION_BUCKET_SIZE");
				var size = int.Parse(option);

				return size;
			}
		}
	}
}
