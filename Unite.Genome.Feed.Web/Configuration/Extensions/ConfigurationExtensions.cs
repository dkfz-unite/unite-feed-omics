using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Cache.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Services.Transcriptomics;
using Unite.Genome.Annotations.Services.Vep;
using Unite.Genome.Feed.Data.Writers.Variants;
using Unite.Genome.Feed.Data.Writers.Transcriptomics;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Annotation;
using Unite.Genome.Feed.Web.Handlers.Indexing;
using Unite.Genome.Feed.Web.Handlers.Submission;
using Unite.Genome.Feed.Web.HostedServices;
using Unite.Genome.Feed.Web.Models.Base.Validators;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;
using Unite.Genome.Indices.Services;

using VariantEntities = Unite.Data.Entities.Genome.Variants;
using VariantModels = Unite.Genome.Feed.Web.Models.Variants;
using TranscriptomicsEntities = Unite.Data.Entities.Genome.Transcriptomics;
using TranscriptomicsModels = Unite.Genome.Feed.Web.Models.Transcriptomics;

using SsmModel = Unite.Genome.Feed.Web.Models.Variants.SSM.VariantModel;
using SsmModelValidator = Unite.Genome.Feed.Web.Models.Variants.SSM.Validators.VariantModelValidator;
using CnvModel = Unite.Genome.Feed.Web.Models.Variants.CNV.VariantModel;
using CnvModelValidator = Unite.Genome.Feed.Web.Models.Variants.CNV.Validators.VariantModelValidator;
using SvModel = Unite.Genome.Feed.Web.Models.Variants.SV.VariantModel;
using SvModelValidator = Unite.Genome.Feed.Web.Models.Variants.SV.Validators.VariantModelValidator;
using BulkExpressionModel = Unite.Genome.Feed.Web.Models.Transcriptomics.BulkExpressionModel;
using BulkExpressionModelValidator = Unite.Genome.Feed.Web.Models.Transcriptomics.Validators.BulkExpressionModelValidator;


namespace Unite.Genome.Feed.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static void Configure(this IServiceCollection services)
    {
        var sqlOptions = new SqlOptions();

        services.AddOptions();
        services.AddDatabase();
        services.AddDatabaseFactory(sqlOptions);
        services.AddIndexServices();
        services.AddValidators();

        services.AddTransient<VariantsDataWriter>();
        services.AddTransient<SsmConsequencesDataWriter>();
        services.AddTransient<CnvConsequencesDataWriter>();
        services.AddTransient<SvConsequencesDataWriter>();
        services.AddTransient<BulkExpressionsDataWriter>();

        // Submission services
        services.AddTransient<VariantsSubmissionService>();
        services.AddTransient<TranscriptomicsSubmissionService>();

        // Annotation services
        services.AddTransient<SsmsAnnotationService>();
        services.AddTransient<CnvsAnnotationService>();
        services.AddTransient<SvsAnnotationService>();
        services.AddTransient<BulkExpressionsAnnotationService>();

        // Task processing services
        services.AddTransient<TasksProcessingService>();

        // Task creation services
        services.AddTransient<SubmissionTaskService>();
        services.AddTransient<GeneIndexingTaskService>();
        services.AddTransient<SsmAnnotationTaskService>();
        services.AddTransient<SsmIndexingTaskService>();
        services.AddTransient<CnvAnnotationTaskService>();
        services.AddTransient<CnvIndexingTaskService>();
        services.AddTransient<SvAnnotationTaskService>();
        services.AddTransient<SvIndexingTaskService>();

        // Submissions hosted services
        services.AddHostedService<SubmissionsHostedService>();
        services.AddTransient<TranscriptomicsSubmissionHandler>();
        services.AddTransient<SsmsSubmissionHandler>();
        services.AddTransient<CnvsSubmissionHandler>();
        services.AddTransient<SvsSubmissionHandler>();

        // Variants annotation hosted service
        services.AddHostedService<VariantsAnnotationHostedService>();
        services.AddTransient<VariantsAnnotationOptions>();
        services.AddTransient<SsmsAnnotationHandler>();
        services.AddTransient<CnvsAnnotationHandler>();
        services.AddTransient<SvsAnnotationHandler>();

        // Variants indexing hosted service
        services.AddHostedService<VariantsIndexingHostedService>();
        services.AddTransient<VariantsIndexingOptions>();
        services.AddTransient<SsmsIndexingHandler>();
        services.AddTransient<CnvsIndexingHandler>();
        services.AddTransient<SvsIndexingHandler>();

        // Genes indexing hosted service
        services.AddHostedService<GenesIndexingHostedService>();
        services.AddTransient<GenesIndexingOptions>();
        services.AddTransient<GenesIndexingHandler>();

        // Variant indexing services
        services.AddTransient<VariantIndexCreationService<VariantEntities.SSM.Variant, VariantEntities.SSM.VariantEntry>>();
        services.AddTransient<VariantIndexCreationService<VariantEntities.CNV.Variant, VariantEntities.CNV.VariantEntry>>();
        services.AddTransient<VariantIndexCreationService<VariantEntities.SV.Variant, VariantEntities.SV.VariantEntry>>();

        // Gene indexing services
        services.AddTransient<GeneIndexCreationService>();
    }


    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddTransient<ApiOptions>();
        services.AddTransient<ISqlOptions, SqlOptions>();
        services.AddTransient<IMongoOptions, MongoOptions>();
        services.AddTransient<IElasticOptions, ElasticOptions>();
        services.AddTransient<IEnsemblVepOptions, EnsemblVepOptions>();
        services.AddTransient<IEnsemblDataOptions, EnsemblDataOptions>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<VariantModels.SequencingDataModel<SsmModel>>, SequencingDataModelValidator<SsmModel, SsmModelValidator>>();
        services.AddTransient<IValidator<VariantModels.SequencingDataModel<CnvModel>>, SequencingDataModelValidator<CnvModel, CnvModelValidator>>();
        services.AddTransient<IValidator<VariantModels.SequencingDataModel<SvModel>>, SequencingDataModelValidator<SvModel, SvModelValidator>>();
        services.AddTransient<IValidator<TranscriptomicsModels.SequencingDataModel<BulkExpressionModel>>, SequencingDataModelValidator<BulkExpressionModel, BulkExpressionModelValidator>>();

        return services;
    }
}
