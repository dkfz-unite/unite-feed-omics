using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Resources;
using Unite.Genome.Annotations.Ensembl.Client.Models;

namespace Unite.Genome.Annotations.Clients.Ensembl
{
    public class EnsemblApiClient
    {
        private const string _lookupOneUrl = @"/lookup/id/{0}";
        private const string _lookupManyUrl = @"/lookup/id";
        private const string _xrefsOneUrl = @"/xrefs/id/{0}";

        private readonly IEnsemblOptions _options;

        public EnsemblApiClient(IEnsemblOptions options)
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
        public async Task<T> Lookup<T>(string ensemblId, bool expand = false) where T : IEnsemblResource
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
        public async Task<T[]> Lookup<T>(IEnumerable<string> ensemblIds, bool expand = false) where T : IEnsemblResource
        {
            using var httpClient = new JsonHttpClient(_options.Host, true);

            var url = _lookupManyUrl;

            if (expand)
            {
                url += "?expand=1";
            }

            var acceptJson = (name: "Accept", value: "application/json");

            var body = new LookupRequestData(ensemblIds);

            var resources = await httpClient.PostAsync<Dictionary<string, T>, LookupRequestData>(url, body, acceptJson);

            return resources.Select(resource => resource.Value).ToArray();
        }

        /// <summary>
        /// Searching references of Ensembl object with given identifier in databases of given types.
        /// </summary>
        /// <typeparam name="T">Object type for decerialization</typeparam>
        /// <param name="ensemblid">Ensembl object identifier</param>
        /// <param name="source">Specific type of external source of information (all by default)</param>
        /// <returns>References of the object stored in ensembl given ensembl databases if were found.</returns>
        public async Task<ReferenceResource[]> Xrefs(string ensemblid, string source = null)
        {
            using var httpClient = new JsonHttpClient(_options.Host, true);

            var url = string.Format(_xrefsOneUrl, ensemblid);

            if (!string.IsNullOrWhiteSpace(source))
            {
                url += $"?external_db={source}";
            }

            var acceptJson = (name: "Accept", value: "application/json");

            var resource = await httpClient.GetAsync<ReferenceResource[]>(url, acceptJson);

            return resource;
        }
    }
}
