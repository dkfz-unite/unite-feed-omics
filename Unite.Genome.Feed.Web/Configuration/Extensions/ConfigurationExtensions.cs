using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;
using Unite.Genome.Annotations.Clients.Vep.Configuration.Options;
using Unite.Genome.Annotations.Services;
using Unite.Genome.Feed.Data.Mutations;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers;
using Unite.Genome.Feed.Web.HostedServices;
using Unite.Genome.Feed.Web.Services.Mutations;
using Unite.Genome.Feed.Web.Services.Mutations.Validators;
using Unite.Genome.Feed.Web.Services.Validation;
using Unite.Genome.Feed.Web.Services;
using Unite.Genome.Indices.Services;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;

namespace Unite.Genome.Feed.Web.Configuration.Extensions
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
            services.AddTransient<IEnsemblOptions, EnsemblOptions>();
        }

        private static void AddDatabases(IServiceCollection services)
        {
            services.AddTransient<DomainDbContext>();
        }

        private static void AddValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<IEnumerable<MutationsModel>>, MutationsModelsValidator>();

            services.AddTransient<IValidationService, ValidationService>();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<MutationDataWriter>();

            services.AddTransient<TasksProcessingService>();

            services.AddTransient<MutationIndexingTaskService>();
            services.AddTransient<IIndexCreationService<Unite.Indices.Entities.Mutations.MutationIndex>, MutationIndexCreationService>();
            services.AddTransient<IIndexingService<Unite.Indices.Entities.Mutations.MutationIndex>, MutationsIndexingService>();

            services.AddTransient<MutationAnnotationTaskService>();
            services.AddTransient<AnnotationService>();
        }

        private static void AddHostedService(IServiceCollection services)
        {
            services.AddTransient<IndexingOptions>();
            services.AddTransient<MutationsIndexingHandler>();
            services.AddHostedService<MutationsIndexingHostedService>();

            services.AddTransient<AnnotationOptions>();
            services.AddTransient<MutationsAnnotationHandler>();
            services.AddHostedService<MutationsAnnotationHostedService>();
        }
    }
}
