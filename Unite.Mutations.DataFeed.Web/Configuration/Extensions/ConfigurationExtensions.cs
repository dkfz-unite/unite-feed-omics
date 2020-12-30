using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;
using Unite.Mutations.DataFeed.Domain.Validation;
using Unite.Mutations.DataFeed.Web.Configuration.Options;
using Unite.Mutations.DataFeed.Web.HostedServices;
using Unite.Mutations.DataFeed.Web.Services;
using Unite.Mutations.DataFeed.Web.Services.Indices;

namespace Unite.Mutations.DataFeed.Web.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddValidation();

            services.AddTransient<UniteDbContext>();

            services.AddTransient<IDataFeedService, DataFeedService>();
            services.AddTransient<IIndexCreationService, IndexCreationService>();
            services.AddTransient<IIndexingService<MutationIndex>, MutationIndexingService>();
            services.AddTransient<ITaskProcessingService, TaskProcessingService>();

            services.AddHostedService<IndexingHostedService>();
        }

        private static void AddOptions(this IServiceCollection services)
        {
            services.AddTransient<IndexingOptions>();
            services.AddTransient<IMySqlOptions, MySqlOptions>();
            services.AddTransient<IElasticOptions, ElasticOptions>();
        }

        private static void AddValidation(this IServiceCollection services)
        {
            services.AddTransient<IValidator<IEnumerable<Domain.Resources.Samples.Sample>>, Domain.Resources.Samples.Validation.SamplesValidator>();
            services.AddTransient<IValidator<IEnumerable<Domain.Resources.Mutations.Mutation>>, Domain.Resources.Mutations.Validation.MutationsValidator>();

            services.AddTransient<IValidationService, ValidationService>();
        }
    }
}
