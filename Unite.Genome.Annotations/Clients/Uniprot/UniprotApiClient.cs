using System.Linq;
using System.Threading.Tasks;
using Unite.Genome.Annotations.Clients.Uniprot.Configuration.Options;
using Unite.Genome.Annotations.Clients.Uniprot.Resources;

namespace Unite.Genome.Annotations.Clients.Uniprot
{
    public class UniprotApiClient
    {
        private const string _proteinUrl = @"/interpro/api/protein/reviewed/{0}/entry/pfam";

        private readonly IUniprotOptions _options;


        public UniprotApiClient(IUniprotOptions options)
        {
            _options = options;
        }


        public async Task<ProteinResource> Protein(string accessionId)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var url = string.Format(_proteinUrl, accessionId);

            var resource = await httpClient.GetAsync<ProteinResource>(url);

            return resource;
        }

        public async Task<ProteinResource[]> Proteins(string[] accessionIds)
        {
            using var httpClient = new JsonHttpClient(_options.Host);

            var tasks = accessionIds.Select(accessionId =>
            {
                var url = string.Format(_proteinUrl, accessionId);
                var task = httpClient.GetAsync<ProteinResource>(url);

                return task;
            });

            var resources = await Task.WhenAll(tasks);

            return resources;
        }
    }
}
