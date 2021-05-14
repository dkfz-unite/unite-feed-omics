using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Mutations.Feed.Mutations.Data;
using Unite.Mutations.Feed.Web.Configuration.Options;
using Unite.Mutations.Feed.Web.Models.Mutations;
using Unite.Mutations.Feed.Web.Models.Mutations.Validators;
using Unite.Mutations.Feed.Web.Models.Validation;
using Unite.Mutations.Feed.Web.Services;

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
        }

        private static void AddOptions(IServiceCollection services)
        {
            services.AddTransient<ISqlOptions, SqlOptions>();
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
            services.AddTransient<MutationIndexingTaskService>();
            services.AddTransient<MutationAnnotationTaskService>();
        }
    }
}
