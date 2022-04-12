using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers;

namespace Unite.Genome.Feed.Web.HostedServices
{
    public class MutationsIndexingHostedService : BackgroundService
    {
        private readonly MutationsIndexingOptions _options;
        private readonly MutationsIndexingHandler _handler;
        private readonly ILogger<MutationsIndexingHostedService> _logger;


        public MutationsIndexingHostedService(
            MutationsIndexingOptions options,
            MutationsIndexingHandler handler,
            ILogger<MutationsIndexingHostedService> logger)
        {
            _options = options;
            _handler = handler;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SSM indexing service started");

            stoppingToken.Register(() => _logger.LogInformation("SSM indexing service stopped"));

            // Delay 5 seconds to let the web api start working
            await Task.Delay(5000, stoppingToken);

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
