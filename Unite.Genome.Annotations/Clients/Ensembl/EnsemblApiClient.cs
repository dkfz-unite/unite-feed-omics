using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Models;
using Unite.Genome.Annotations.Clients.Ensembl.Resources;

namespace Unite.Genome.Annotations.Clients.Ensembl;

public class EnsemblApiClient
{
    private const int _bucketSize = 200;

    private const string _lookupOneUrl = @"/lookup/id/{0}";
    private const string _lookupManyUrl = @"/lookup/id";
    private const string _xrefsOneUrl = @"/xrefs/id/{0}";

    private readonly IEnsemblDataOptions _options;

    public EnsemblApiClient(IEnsemblDataOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Searching an object with given identifier in Ensembl database.
    /// </summary>
    /// <typeparam name="T">Object type for deserialization</typeparam>
    /// <param name="ensemblId">Ensembl object identifier</param>
    /// <param name="expand">Expand parameter value (setting this parameter to 'true' will force Ensembl to return all nested data)</param>
    /// <returns>Ensembl object mapped to given type if was found.</returns>
    public async Task<T> Lookup<T>(string ensemblId, bool expand = false) where T : LookupResource
    {
        using var httpClient = new JsonHttpClient(_options.Host, true);

        var url = string.Format(_lookupOneUrl, ensemblId);

        if (expand)
        {
            url += "?expand=1";
        }

        var acceptJson = (name: "Accept", value: "application/json");

        var resource = await httpClient.GetAsync<T>(url, acceptJson);

        return resource;
    }

    /// <summary>
    /// Searching objects with given identifiers in Ensembl database.
    /// </summary>
    /// <typeparam name="T">Object type for deserialization</typeparam>
    /// <param name="ensemblIds">Ensembl object identifiers</param>
    /// <param name="expand">Expand parameter value (setting this parameter to 'true' will force Ensembl to return all nested data)</param>
    /// <returns>Ensembl objects mapped to an array of given type if were found.</returns>
    public async Task<T[]> Lookup<T>(IEnumerable<string> ensemblIds, bool expand = false) where T : LookupResource
    {
        using var httpClient = new JsonHttpClient(_options.Host, true);

        var url = _lookupManyUrl;

        if (expand)
        {
            url += "?expand=1";
        }

        var acceptJson = (name: "Accept", value: "application/json");

        var queue = new Queue<string>(ensemblIds);

        var results = new List<T>();

        while (queue.Any())
        {
            var tasks = new List<Task<Dictionary<string, T>>>();

            for (var i = 0; i < 5 && queue.Any(); i++)
            {
                var chunk = queue.Dequeue(_bucketSize);
                var body = new LookupRequestData(chunk);
                var task = httpClient.PostAsync<Dictionary<string, T>, LookupRequestData>(url, body, acceptJson);

                tasks.Add(task);
            }

            var responses = await Task.WhenAll(tasks);

            foreach (var response in responses)
            {
                results.AddRange(response.Select(resource => resource.Value));
            }
        }

        return results.ToArray();
    }

    /// <summary>
    /// Searching references of Ensembl object with given identifier in databases of given types.
    /// </summary>
    /// <typeparam name="T">Object type for decerialization</typeparam>
    /// <param name="ensemblId">Ensembl object identifier</param>
    /// <param name="source">Specific type of external source of information (all by default)</param>
    /// <returns>References of the object stored in ensembl given ensembl databases if were found.</returns>
    public async Task<ReferenceResource[]> Xrefs(string ensemblId, string source = null)
    {
        using var httpClient = new JsonHttpClient(_options.Host, true);

        var url = string.Format(_xrefsOneUrl, ensemblId);

        if (!string.IsNullOrWhiteSpace(source))
        {
            url += $"?external_db={source}";
        }

        var acceptJson = (name: "Accept", value: "application/json");

        var resource = await httpClient.GetAsync<ReferenceResource[]>(url, acceptJson);

        return resource;
    }
}

internal static class IEnumerableEstensions
{
    public static IEnumerable<T> Dequeue<T>(this Queue<T> queue, int chunkSize)
    {
        for (int i = 0; i < chunkSize && queue.Count > 0; i++)
        {
            yield return queue.Dequeue();
        }
    }
}
