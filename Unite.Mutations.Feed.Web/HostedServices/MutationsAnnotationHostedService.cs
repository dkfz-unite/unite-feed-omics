using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unite.Mutations.Feed.Web.Configuration.Options;
using Unite.Mutations.Feed.Web.Handlers;

namespace Unite.Mutations.Feed.Web.HostedServices
{
    public class MutationsAnnotationHostedService : BackgroundService
    {
        private readonly AnnotationOptions _options;
        private readonly MutationsAnnotationHandler _handler;
        private readonly ILogger _logger;


        public MutationsAnnotationHostedService(
            AnnotationOptions options,
            MutationsAnnotationHandler handler,
            ILogger<MutationsAnnotationHostedService> logger)
        {
            _options = options;
            _handler = handler;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Annotation service started");

            stoppingToken.Register(() => _logger.LogInformation("Annotation service stopped"));

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
