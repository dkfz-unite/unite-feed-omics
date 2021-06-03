using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;
using Unite.Mutations.Annotations.Vep.Configuration.Options;
using Unite.Mutations.Annotations.Vep.Services;
using Unite.Mutations.Feed.Data.Mutations;
using Unite.Mutations.Feed.Web.Configuration.Options;
using Unite.Mutations.Feed.Web.Handlers;
using Unite.Mutations.Feed.Web.HostedServices;
using Unite.Mutations.Feed.Web.Models.Mutations;
using Unite.Mutations.Feed.Web.Models.Mutations.Validators;
using Unite.Mutations.Feed.Web.Models.Validation;
using Unite.Mutations.Feed.Web.Services;
using Unite.Mutations.Indices.Services;

namespace Unite.Mutations.Feed.Web.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void Configure(this IServiceCollection services)
        {
            AddOptions(services);
            AddDatabases(services);
            AddValidation(services);
            AddServices(services);
            AddHostedService(services);
        }

        private static void AddOptions(IServiceCollection services)
        {
            services.AddTransient<ISqlOptions, SqlOptions>();
            services.AddTransient<IElasticOptions, ElasticOptions>();
            services.AddTransient<IVepOptions, VepOptions>();
        }

        private static void AddDatabases(IServiceCollection services)
        {
            services.AddTransient<UniteDbContext>();
        }

        private static void AddValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<IEnumerable<MutationsModel>>, MutationsModelsValidator>();

            services.AddTransient<IValidationService, ValidationService>();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<MutationDataWriter>();

            services.AddTransient<TaskProcessingService>();

            services.AddTransient<MutationIndexingTaskService>();
            services.AddTransient<IIndexCreationService<MutationIndex>, MutationIndexCreationService>();
            services.AddTransient<IIndexingService<MutationIndex>, MutationIndexingService>();

            services.AddTransient<MutationAnnotationTaskService>();
            services.AddTransient<VepAnnotationService>();
        }

        private static void AddHostedService(IServiceCollection services)
        {
            services.AddTransient<IndexingOptions>();
            services.AddTransient<IndexingHandler>();
            services.AddHostedService<IndexingHostedService>();

            services.AddTransient<AnnotationOptions>();
            services.AddTransient<AnnotationHandler>();
            services.AddHostedService<AnnotationHostedService>();
        }
    }
}
