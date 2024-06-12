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
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Annotation;
using Unite.Genome.Feed.Web.Handlers.Indexing;
using Unite.Genome.Feed.Web.Handlers.Submission;
using Unite.Genome.Feed.Web.Workers;
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
using BulkExpModel = Unite.Genome.Feed.Web.Models.Rna.ExpressionModel;
using BulkExpModelValidator = Unite.Genome.Feed.Web.Models.Rna.Validators.ExpressionModelValidator;
using CellExpModel = Unite.Genome.Feed.Web.Models.RnaSc.ExpressionModel;
using CellExpModelValidator = Unite.Genome.Feed.Web.Models.RnaSc.Validators.ExpressionModelValidator;


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

        services.AddTransient<Data.Writers.Dna.AnalysisWriter>();
        services.AddTransient<Data.Writers.Dna.EffectsSsmWriter>();
        services.AddTransient<Data.Writers.Dna.EffectsCnvWriter>();
        services.AddTransient<Data.Writers.Dna.EffectsSvWriter>();
        services.AddTransient<Data.Writers.Rna.AnalysisWriter>();
        services.AddTransient<Data.Writers.RnaSc.AnalysisWriter>();

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
        services.AddHostedService<SubmissionsWorker>();
        services.AddTransient<BulkGeneExpSubmissionHandler>();
        services.AddTransient<CellGeneExpSubmissionHandler>();
        services.AddTransient<SsmsSubmissionHandler>();
        services.AddTransient<CnvsSubmissionHandler>();
        services.AddTransient<SvsSubmissionHandler>();

        // Variants annotation hosted service
        services.AddHostedService<VariantsAnnotationWorker>();
        services.AddTransient<VariantsAnnotationOptions>();
        services.AddTransient<SsmsAnnotationHandler>();
        services.AddTransient<CnvsAnnotationHandler>();
        services.AddTransient<SvsAnnotationHandler>();

        // Variants indexing hosted service
        services.AddHostedService<VariantsIndexingWorker>();
        services.AddTransient<VariantsIndexingOptions>();
        services.AddTransient<SsmsIndexingHandler>();
        services.AddTransient<CnvsIndexingHandler>();
        services.AddTransient<SvsIndexingHandler>();

        // Genes indexing hosted service
        services.AddHostedService<GenesIndexingWorker>();
        services.AddTransient<GenesIndexingOptions>();
        services.AddTransient<GenesIndexingHandler>();

        // Variant indexing services
        services.AddTransient<VariantIndexingCache<DnaEntities.Ssm.Variant, DnaEntities.Ssm.VariantEntry>>();
        services.AddTransient<VariantIndexingCache<DnaEntities.Cnv.Variant, DnaEntities.Cnv.VariantEntry>>();
        services.AddTransient<VariantIndexingCache<DnaEntities.Sv.Variant, DnaEntities.Sv.VariantEntry>>();

        // Gene indexing services
        services.AddTransient<GenesIndexingCache>();
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
        services.AddTransient<IValidator<AnalysisModel<SsmModel>>, AnalysisModelValidator<SsmModel, SsmModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<CnvModel>>, AnalysisModelValidator<CnvModel, CnvModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<SvModel>>, AnalysisModelValidator<SvModel, SvModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<BulkExpModel>>, AnalysisModelValidator<BulkExpModel, BulkExpModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<CellExpModel>>, AnalysisModelValidator<CellExpModel, CellExpModelValidator>>();

        return services;
    }
}
