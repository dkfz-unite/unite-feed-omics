using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Resources;

namespace Unite.Genome.Annotations.Clients.Ensembl;

public class EnsemblApiClient1
{
    private const int _threadsNumber = 5;
    private const int _bucketSize = 200;

    private const string _genesUrl = @"/api/genes";
    private const string _transcriptsUrl = @"/api/transcripts";
    private const string _proteinsUrl = @"/api/proteins";

    private readonly IEnsemblOptions _options;


    public EnsemblApiClient1(IEnsemblOptions options)
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
    public async Task<T> Find<T>(string ensemblId, bool length = true, bool expand = false) where T : LookupResource
    {
        using var httpClient = new JsonHttpClient(_options.Host);

        var url = $"{GetUrl<T>()}/id/{ensemblId}";

        url += GetArguments(length, expand);

        var resource = await httpClient.GetAsync<T>(url);

        return resource;
    }

    /// <summary>
    /// Searching objects with given identifiers in Ensembl database.
    /// </summary>
    /// <typeparam name="T">Object type for deserialization</typeparam>
    /// <param name="ensemblIds">Ensembl object identifiers</param>
    /// <param name="expand">Expand parameter value (setting this parameter to 'true' will force Ensembl to return all nested data)</param>
    /// <returns>Ensembl objects mapped to an array of given type if were found.</returns>
    public async Task<T[]> Find<T>(IEnumerable<string> ensemblIds, bool length = true, bool expand = false) where T : LookupResource
    {
        using var httpClient = new JsonHttpClient(_options.Host);

        var url = $"{GetUrl<T>()}/id";

        url += GetArguments(length, expand);

        var queue = new Queue<string>(ensemblIds);

        var resources = new List<T>();

        while (queue.Any())
        {
            var tasks = new List<Task<T[]>>();

            for (var i = 0; i < _threadsNumber && queue.Any(); i++)
            {
                var body = queue.Dequeue(_bucketSize).ToArray();
                var task = httpClient.PostAsync<T[], string[]>(url, body);

                tasks.Add(task);
            }

            var responses = await Task.WhenAll(tasks);

            foreach (var response in responses)
            {
                resources.AddRange(response);
            }
        }

        return resources.ToArray();
    }

    /// <summary>
    /// Searching an object with given symbol in Ensembl database.
    /// </summary>
    /// <typeparam name="T">Object type for deserialization</typeparam>
    /// <param name="ensemblId">Ensembl object symbol</param>
    /// <param name="expand">Expand parameter value (setting this parameter to 'true' will force Ensembl to return all nested data)</param>
    /// <returns>Ensembl object mapped to given type if was found.</returns>
    public async Task<T> FindByName<T>(string symbol, bool length = true, bool expand = false) where T : LookupResource
    {
        using var httpClient = new JsonHttpClient(_options.Host);

        var url = $"{GetUrl<T>()}/symbol/{symbol}";

        url += GetArguments(length, expand);

        var resource = await httpClient.GetAsync<T>(url);

        return resource;
    }

    /// <summary>
    /// Searching objects with given symbols in Ensembl database.
    /// </summary>
    /// <typeparam name="T">Object type for deserialization</typeparam>
    /// <param name="symbols">Ensembl object symbols</param>
    /// <param name="expand">Expand parameter value (setting this parameter to 'true' will force Ensembl to return all nested data)</param>
    /// <returns>Ensembl objects mapped to an array of given type if were found.</returns>
    public async Task<T[]> FindByName<T>(IEnumerable<string> symbols, bool length = true, bool expand = false) where T : LookupResource
    {
        using var httpClient = new JsonHttpClient(_options.Host);

        var url = $"{GetUrl<T>()}/symbol";

        url += GetArguments(length, expand);

        var queue = new Queue<string>(symbols);

        var resources = new List<T>();

        while (queue.Any())
        {
            var tasks = new List<Task<T[]>>();

            for (var i = 0; i < _threadsNumber && queue.Any(); i++)
            {
                var body = queue.Dequeue(_bucketSize).ToArray();
                var task = httpClient.PostAsync<T[], string[]>(url, body);

                tasks.Add(task);
            }

            var responses = await Task.WhenAll(tasks);

            foreach (var response in responses)
            {
                resources.AddRange(response);
            }
        }

        return resources.ToArray();
    }


    private static string GetArguments(bool length, bool expand)
    {
        var arguments = new string[]
        {
            $"length={length}",
            $"expand={expand}"
        };

        return $"?{string.Join('&', arguments)}";
    }

    private static string GetUrl<T>() where T : LookupResource
    {
        return typeof(T) == typeof(GeneResource) ? $"{_genesUrl}"
             : typeof(T) == typeof(TranscriptResource) ? $"{_transcriptsUrl}"
             : typeof(T) == typeof(ProteinResource) ? $"{_proteinsUrl}"
             : throw new NotImplementedException();
    }
}
