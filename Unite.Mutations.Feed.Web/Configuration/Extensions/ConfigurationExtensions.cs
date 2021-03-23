using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;
using Unite.Mutations.Feed.Annotations;
using Unite.Mutations.Feed.Annotations.VEP;
using Unite.Mutations.Feed.Annotations.VEP.Configuration.Options;
using Unite.Mutations.Feed.Annotations.VEP.Resources;
using Unite.Mutations.Feed.Data.Services;
using Unite.Mutations.Feed.Data.Services.Annotations;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep;
using Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep.Audit;
using Unite.Mutations.Feed.Data.Services.Mutations;
using Unite.Mutations.Feed.Data.Services.Mutations.Models;
using Unite.Mutations.Feed.Data.Services.Mutations.Models.Audit;
using Unite.Mutations.Feed.Indices.Services;
using Unite.Mutations.Feed.Web.Configuration.Options;
using Unite.Mutations.Feed.Web.Handlers;
using Unite.Mutations.Feed.Web.HostedServices;
using Unite.Mutations.Feed.Web.Resources.Mutations;
using Unite.Mutations.Feed.Web.Resources.Mutations.Validators;
using Unite.Mutations.Feed.Web.Resources.Validation;

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
            AddHostedServices(services);
        }

        private static void AddOptions(IServiceCollection services)
        {
            services.AddTransient<ISqlOptions, SqlOptions>();
            services.AddTransient<IElasticOptions, ElasticOptions>();
            services.AddTransient<IVepOptions, VepOptions>();
            services.AddTransient<AnnotationOptions>();
            services.AddTransient<IndexingOptions>();
        }

        private static void AddDatabases(IServiceCollection services)
        {
            services.AddTransient<UniteDbContext>();
        }

        private static void AddValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<IEnumerable<MutationsResource>>, MutationResourcesValidator>();

            services.AddTransient<IValidationService, ValidationService>();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IDataService<MutationsModel, MutationsUploadAudit>, MutationsDataService>();
            services.AddTransient<IDataService<AnnotationModel, AnnotationsUploadAudit>, VepAnnotationsDataService>();
            services.AddTransient<IAnnotationApiClient<AnnotationResource>, VepAnnotationApiClient>();
            services.AddTransient<MutationIndexingTaskService>();
            services.AddTransient<VepAnnotationTaskService>();
            services.AddTransient<MutationIndexCreationService>();
            services.AddTransient<IIndexingService<MutationIndex>, MutationIndexingService>();
        }

        private static void AddHostedServices(IServiceCollection services)
        {
            services.AddHostedService<IndexingHostedService>();
            services.AddHostedService<AnnotationHostedService>();

            services.AddTransient<IndexingHandler>();
            services.AddTransient<AnnotationHandler>();
        }
    }
}
