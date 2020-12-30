using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unite.Mutations.DataFeed.Web.Configuration.Options;
using Unite.Mutations.DataFeed.Web.Services;

namespace Unite.Mutations.DataFeed.Web.HostedServices
{
    public class IndexingHostedService : BackgroundService
    {
        private readonly IndexingOptions _options;
        private readonly ITaskProcessingService _taskProcessingService;
        private readonly ILogger<IndexingHostedService> _logger;

        public IndexingHostedService(
            IndexingOptions options,
            ITaskProcessingService taskProcessingService,
            ILogger<IndexingHostedService> logger)
        {
            _options = options;
            _taskProcessingService = taskProcessingService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Processing service started");

            stoppingToken.Register(() => _logger.LogInformation("Processing service stopped"));

            while (!stoppingToken.IsCancellationRequested)
            {
                _taskProcessingService.ProcessIndexingTasks(_options.BucketSize);

                await Task.Delay(_options.Interval, stoppingToken);
            }
        }
    }
}
