using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unite.Mutations.Feed.Web.Configuration.Options;
using Unite.Mutations.Feed.Web.Handlers;

namespace Unite.Mutations.Feed.Web.HostedServices
{
    public class IndexingHostedService : BackgroundService
    {
        private readonly IndexingOptions _options;
        private readonly IndexingHandler _handler;
        private readonly ILogger<IndexingHostedService> _logger;

        public IndexingHostedService(
            IndexingOptions options,
            IndexingHandler handler,
            ILogger<IndexingHostedService> logger)
        {
            _options = options;
            _handler = handler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Processing service started");

            stoppingToken.Register(() => _logger.LogInformation("Processing service stopped"));

            while (!stoppingToken.IsCancellationRequested)
            {
                _handler.Handle(_options.BucketSize);

                await Task.Delay(_options.Interval, stoppingToken);
            }
        }
    }
}
