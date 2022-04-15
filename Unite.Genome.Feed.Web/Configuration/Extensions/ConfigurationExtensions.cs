using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Vep.Configuration.Options;
using Unite.Genome.Annotations.Services;
using Unite.Genome.Feed.Data.Mutations;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers;
using Unite.Genome.Feed.Web.HostedServices;
using Unite.Genome.Feed.Web.Services;
using Unite.Genome.Feed.Web.Services.Mutations;
using Unite.Genome.Feed.Web.Services.Mutations.Validators;
using Unite.Genome.Indices.Services;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Genome.Feed.Web.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void Configure(this IServiceCollection services)
        {
            services.AddTransient<ISqlOptions, SqlOptions>();
            services.AddTransient<IElasticOptions, ElasticOptions>();
            services.AddTransient<IVepOptions, VepOptions>();
            services.AddTransient<IEnsemblOptions, EnsemblOptions>();

            services.AddTransient<IValidator<MutationsModel[]>, MutationsModelsValidator>();

            services.AddTransient<DomainDbContext>();
            services.AddTransient<MutationDataWriter>();

            services.AddTransient<AnnotationService>();
            services.AddTransient<GeneIndexingTaskService>();
            services.AddTransient<MutationAnnotationTaskService>();
            services.AddTransient<MutationIndexingTaskService>();
            services.AddTransient<TasksProcessingService>();

            services.AddHostedService<MutationsAnnotationHostedService>();
            services.AddTransient<MutationsAnnotationOptions>();
            services.AddTransient<MutationsAnnotationHandler>();

            services.AddHostedService<MutationsIndexingHostedService>();
            services.AddTransient<MutationsIndexingOptions>();
            services.AddTransient<MutationsIndexingHandler>();
            services.AddTransient<IIndexCreationService<Unite.Indices.Entities.Mutations.MutationIndex>, MutationIndexCreationService>();
            services.AddTransient<IIndexingService<Unite.Indices.Entities.Mutations.MutationIndex>, MutationsIndexingService>();

            services.AddHostedService<GenesIndexingHostedService>();
            services.AddTransient<GenesIndexingOptions>();
            services.AddTransient<GenesIndexingHandler>();
            services.AddTransient<IIndexCreationService<Unite.Indices.Entities.Genes.GeneIndex>, GeneIndexCreationService>();
            services.AddTransient<IIndexingService<Unite.Indices.Entities.Genes.GeneIndex>, GenesIndexingService>();
        }
    }
}
