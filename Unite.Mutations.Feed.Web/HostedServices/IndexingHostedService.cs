using System;
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
            _logger.LogInformation("Indexing service started");

            stoppingToken.Register(() => _logger.LogInformation("Indexing service stopped"));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _handler.Handle(_options.BucketSize);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message);

                    if (exception.InnerException != null)
                    {
                        _logger.LogError(exception.InnerException.Message);
                    }
                }
                finally
                {
                    await Task.Delay(_options.Interval, stoppingToken);
                }
            }
        }
    }
}
