namespace Unite.Genome.Feed.Data.Extensions;

internal static class HashSetExtensions
{
	public static void AddRange<T>(this HashSet<T> source, IEnumerable<T> values)
	{
		foreach (var value in values)
		{
			source.Add(value);
		}
	}
}
