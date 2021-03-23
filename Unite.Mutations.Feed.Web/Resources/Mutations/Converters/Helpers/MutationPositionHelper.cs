namespace Unite.Mutations.Feed.Web.Resources.Mutations.Converters.Helpers
{
    public class MutationPositionHelper
    {
        /// <summary>
        /// Retreives start position of mutation.
        /// </summary>
        /// <param name="position">Position string ('1234567890' or '1234567890-1234567890')</param>
        /// <returns>Start position of mutation.</returns>
        public static int GetMutationStart(string position)
        {
            if (position.Contains('-'))
            {
                return int.Parse(position.Split('-')[0]);
            }
            else
            {
                return int.Parse(position);
            }
        }

        /// <summary>
        /// Retreives end position of mutation.
        /// </summary>
        /// <param name="position">Position string ('1234567890' or '1234567890-1234567890')</param>
        /// <returns>End position of mutation.</returns>
        public static int GetMutationEnd(string position)
        {
            if (position.Contains('-'))
            {
                return int.Parse(position.Split('-')[1]);
            }
            else
            {
                return int.Parse(position);
            }
        }
    }
}
