using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Cache.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Services.Rna;
using Unite.Genome.Annotations.Services.Vep;
using Unite.Genome.Feed.Data.Writers.Dna;
using Unite.Genome.Feed.Data.Writers.Rna;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Annotation;
using Unite.Genome.Feed.Web.Handlers.Indexing;
using Unite.Genome.Feed.Web.Handlers.Submission;
using Unite.Genome.Feed.Web.HostedServices;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Base.Validators;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;
using Unite.Genome.Indices.Services;

using DnaEntities = Unite.Data.Entities.Genome.Analysis.Dna;

using SsmModel = Unite.Genome.Feed.Web.Models.Dna.Ssm.VariantModel;
using SsmModelValidator = Unite.Genome.Feed.Web.Models.Dna.Ssm.Validators.VariantModelValidator;
using CnvModel = Unite.Genome.Feed.Web.Models.Dna.Cnv.VariantModel;
using CnvModelValidator = Unite.Genome.Feed.Web.Models.Dna.Cnv.Validators.VariantModelValidator;
using SvModel = Unite.Genome.Feed.Web.Models.Dna.Sv.VariantModel;
using SvModelValidator = Unite.Genome.Feed.Web.Models.Dna.Sv.Validators.VariantModelValidator;
using BulkExpressionModel = Unite.Genome.Feed.Web.Models.Rna.BulkExpressionModel;
using BulkExpressionModelValidator = Unite.Genome.Feed.Web.Models.Rna.Validators.BulkExpressionModelValidator;
using CellExpressionModel = Unite.Genome.Feed.Web.Models.Rna.CellExpressionModel;
using CellExpressionModelValidator = Unite.Genome.Feed.Web.Models.Rna.Validators.CellExpressionModelValidator;


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
        services.AddTransient<EffectsDataSsmWriter>();
        services.AddTransient<EffectsDataCnvWriter>();
        services.AddTransient<EffectsDataSvWriter>();
        services.AddTransient<BulkExpDataWriter>();
        services.AddTransient<CellExpDataWriter>();

        // Submission services
        services.AddTransient<DnaSubmissionService>();
        services.AddTransient<RnaSubmissionService>();

        // Annotation services
        services.AddTransient<SsmsAnnotationService>();
        services.AddTransient<CnvsAnnotationService>();
        services.AddTransient<SvsAnnotationService>();
        services.AddTransient<ExpressionsAnnotationService>();

        // Task processing services
        services.AddTransient<TasksProcessingService>();

        // Task creation services
        services.AddTransient<SubmissionTaskService>();
        services.AddTransient<SampleIndexingTaskService>();
        services.AddTransient<GeneIndexingTaskService>();
        services.AddTransient<SsmAnnotationTaskService>();
        services.AddTransient<SsmIndexingTaskService>();
        services.AddTransient<CnvAnnotationTaskService>();
        services.AddTransient<CnvIndexingTaskService>();
        services.AddTransient<SvAnnotationTaskService>();
        services.AddTransient<SvIndexingTaskService>();

        // Submissions hosted services
        services.AddHostedService<SubmissionsHostedService>();
        services.AddTransient<BulkRnaSubmissionHandler>();
        services.AddTransient<CellRnaSubmissionHandler>();
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
        services.AddTransient<VariantIndexCreationService<DnaEntities.Ssm.Variant, DnaEntities.Ssm.VariantEntry>>();
        services.AddTransient<VariantIndexCreationService<DnaEntities.Cnv.Variant, DnaEntities.Cnv.VariantEntry>>();
        services.AddTransient<VariantIndexCreationService<DnaEntities.Sv.Variant, DnaEntities.Sv.VariantEntry>>();

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
        services.AddTransient<IValidator<SeqDataModel<SsmModel>>, SeqDataModelValidator<SsmModel, SsmModelValidator>>();
        services.AddTransient<IValidator<SeqDataModel<CnvModel>>, SeqDataModelValidator<CnvModel, CnvModelValidator>>();
        services.AddTransient<IValidator<SeqDataModel<SvModel>>, SeqDataModelValidator<SvModel, SvModelValidator>>();
        services.AddTransient<IValidator<SeqDataModel<BulkExpressionModel>>, SeqDataModelValidator<BulkExpressionModel, BulkExpressionModelValidator>>();
        services.AddTransient<IValidator<SeqDataModel<CellExpressionModel>>, SeqDataModelValidator<CellExpressionModel, CellExpressionModelValidator>>();

        return services;
    }
}
