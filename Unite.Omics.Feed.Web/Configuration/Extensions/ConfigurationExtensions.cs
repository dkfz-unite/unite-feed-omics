using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Cache.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Rna;
using Unite.Omics.Annotations.Services.Vep;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Annotation;
using Unite.Omics.Feed.Web.Handlers.Indexing;
using Unite.Omics.Feed.Web.Handlers.Submission;
using Unite.Omics.Feed.Web.Workers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Services.Annotation;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions;
using Unite.Omics.Indices.Services;

using DnaEntities = Unite.Data.Entities.Omics.Analysis.Dna;

using SmModel = Unite.Omics.Feed.Web.Models.Dna.Sm.VariantModel;
using SmModelValidator = Unite.Omics.Feed.Web.Models.Dna.Sm.Validators.VariantModelValidator;
using CnvModel = Unite.Omics.Feed.Web.Models.Dna.Cnv.VariantModel;
using CnvModelValidator = Unite.Omics.Feed.Web.Models.Dna.Cnv.Validators.VariantModelValidator;
using SvModel = Unite.Omics.Feed.Web.Models.Dna.Sv.VariantModel;
using SvModelValidator = Unite.Omics.Feed.Web.Models.Dna.Sv.Validators.VariantModelValidator;
using BulkExpModel = Unite.Omics.Feed.Web.Models.Rna.ExpressionModel;
using BulkExpModelValidator = Unite.Omics.Feed.Web.Models.Rna.Validators.ExpressionModelValidator;


namespace Unite.Omics.Feed.Web.Configuration.Extensions;

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

        services.AddTransient<Data.Writers.SampleWriter>();
        services.AddTransient<Data.Writers.Dna.AnalysisWriter>();
        services.AddTransient<Data.Writers.Dna.EffectsSmWriter>();
        services.AddTransient<Data.Writers.Dna.EffectsCnvWriter>();
        services.AddTransient<Data.Writers.Dna.EffectsSvWriter>();
        services.AddTransient<Data.Writers.Rna.AnalysisWriter>();
        services.AddTransient<Data.Writers.RnaSc.AnalysisWriter>();

        // Submission services
        services.AddTransient<DnaSubmissionService>();
        services.AddTransient<MethSubmissionService>();
        services.AddTransient<RnaSubmissionService>();
        services.AddTransient<RnaScSubmissionService>();

        // Annotation services
        services.AddTransient<SmsAnnotationService>();
        services.AddTransient<CnvsAnnotationService>();
        services.AddTransient<SvsAnnotationService>();
        services.AddTransient<ExpressionsAnnotationService>();

        // Task processing services
        services.AddTransient<TasksProcessingService>();

        // Task creation services
        services.AddTransient<SubmissionTaskService>();
        services.AddTransient<SampleIndexingTaskService>();
        services.AddTransient<GeneIndexingTaskService>();
        services.AddTransient<SmAnnotationTaskService>();
        services.AddTransient<SmIndexingTaskService>();
        services.AddTransient<CnvAnnotationTaskService>();
        services.AddTransient<CnvIndexingTaskService>();
        services.AddTransient<SvAnnotationTaskService>();
        services.AddTransient<SvIndexingTaskService>();

        // Submissions hosted services
        services.AddHostedService<SubmissionsWorker>();
        services.AddTransient<BulkGeneExpSubmissionHandler>();
        services.AddTransient<CellGeneExpSubmissionHandler>();
        services.AddTransient<SmsSubmissionHandler>();
        services.AddTransient<CnvsSubmissionHandler>();
        services.AddTransient<SvsSubmissionHandler>();
        services.AddTransient<MethSubmissionHandler>();

        // Variants annotation hosted service
        services.AddHostedService<VariantsAnnotationWorker>();
        services.AddTransient<VariantsAnnotationOptions>();
        services.AddTransient<SmsAnnotationHandler>();
        services.AddTransient<CnvsAnnotationHandler>();
        services.AddTransient<SvsAnnotationHandler>();

        // Variants indexing hosted service
        services.AddHostedService<VariantsIndexingWorker>();
        services.AddTransient<VariantsIndexingOptions>();
        services.AddTransient<SmsIndexingHandler>();
        services.AddTransient<CnvsIndexingHandler>();
        services.AddTransient<SvsIndexingHandler>();

        // Genes indexing hosted service
        services.AddHostedService<GenesIndexingWorker>();
        services.AddTransient<GenesIndexingOptions>();
        services.AddTransient<GenesIndexingHandler>();

        // Variant indexing services
        services.AddTransient<VariantIndexingCache<DnaEntities.Sm.Variant, DnaEntities.Sm.VariantEntry>>();
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
        services.AddTransient<IValidator<SampleModel>, SampleModelValidator>();
        services.AddTransient<IValidator<AnalysisModel<SmModel>>, AnalysisModelValidator<SmModel, SmModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<CnvModel>>, AnalysisModelValidator<CnvModel, CnvModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<SvModel>>, AnalysisModelValidator<SvModel, SvModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<BulkExpModel>>, AnalysisModelValidator<BulkExpModel, BulkExpModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<EmptyModel>>, AnalysisModelValidator<EmptyModel, EmptyModelValidator>>();

        services.AddTransient<IValidator<AnalysisForm<SmModel>>, AnalysisFormValidator<SmModel>>();

        return services;
    }
}
