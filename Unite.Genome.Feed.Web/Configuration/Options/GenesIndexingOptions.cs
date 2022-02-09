using System;

namespace Unite.Genome.Feed.Web.Configuration.Options
{
    public class GenesIndexingOptions
    {
        /// <summary>
        /// Indexing interval in milliseconds
        /// </summary>
        public int Interval
        {
            get
            {
                var option = Environment.GetEnvironmentVariable("UNITE_GENES_INDEXING_INTERVAL");
                var seconds = int.Parse(option);

                return seconds * 1000;
            }
        }

        /// <summary>
        /// Indexing bucket size
        /// </summary>
        public int BucketSize
        {
            get
            {
                var option = Environment.GetEnvironmentVariable("UNITE_GENES_INDEXING_BUCKET_SIZE");
                var size = int.Parse(option);

                return size;
            }
        }
    }
}
