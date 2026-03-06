using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Cache.Configuration.Options;
using Unite.Indices.Entities.CnvProfiles;
using Unite.Indices.Entities.Genes;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Rna;
using Unite.Omics.Annotations.Services.Vep;
using Unite.Omics.Feed.Data.Writers.Dna;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers;
using Unite.Omics.Feed.Web.Handlers.Annotation;
using Unite.Omics.Feed.Web.Handlers.Indexing;
using Unite.Omics.Feed.Web.Handlers.Submission;
using Unite.Omics.Feed.Web.Workers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Services.Annotation;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;
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
        //services.AddTransient<CnvIndexingTaskService>();
        services.AddTransient<SvAnnotationTaskService>();
        services.AddTransient<SvIndexingTaskService>();
        services.AddTransient<CnvProfileIndexingTaskService>();

        // Submissions hosted services
        services.AddHostedService<SubmissionsWorker>();
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<RnaSubmissionHandler>(sp, HandlerPriority.Highest));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<RnaExpSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<RnascSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<RnascExpSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<DnaSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<DnaSmSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<DnaCnvSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<DnaSvSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<MethSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<MethLvlSubmissionHandler>(sp, HandlerPriority.Normal));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<CnvProfileSubmissionHandler>(sp, HandlerPriority.Normal));
        
        //Indexing Handlers
        services.AddHostedService<IndexingWorker>();
        services.AddTransient<IIndexingHandler, GenesIndexingHandler>();
        services.AddTransient<IIndexingHandler, CnvsIndexingHandler>();
        services.AddTransient<IIndexingHandler, SmsIndexingHandler>();
        services.AddTransient<IIndexingHandler, SvsIndexingHandler>();
        services.AddTransient<IIndexingHandler, CnvProfilesIndexingHandler>();
        
        //Index Creators
        services.AddTransient<CnvIndexEntityBuilder>();
        services.AddTransient<SmIndexEntityBuilder>();
        services.AddTransient<SvIndexEntityBuilder>();
        services.AddTransient<GeneIndexEntityBuilder>();
        services.AddTransient<CnvProfileIndexEntityBuilder>();

        // Variants annotation hosted service
        services.AddHostedService<VariantsAnnotationWorker>();
        services.AddTransient<VariantsAnnotationOptions>();
        services.AddTransient<SmsAnnotationHandler>();
        services.AddTransient<CnvsAnnotationHandler>();
        services.AddTransient<SvsAnnotationHandler>();

        // Variants indexing hosted service
        services.AddTransient<VariantsIndexingOptions>();
        services.AddTransient<SmsIndexingHandler>();
        services.AddTransient<CnvsIndexingHandler>();
        services.AddTransient<SvsIndexingHandler>();

        // Genes indexing hosted service
        services.AddTransient<GenesIndexingOptions>();
        services.AddTransient<GenesIndexingHandler>();
        
        //Submission repositories
        services.AddTransient<CnvProfileSubmissionRepository>();
        services.AddTransient<CnvSubmissionRepository>();
        services.AddTransient<SampleSubmissionRepository>();
        services.AddTransient<SmSubmissionRepository>();
        services.AddTransient<SvSubmissionRepository>();
        
        services.AddTransient<Submissions.Repositories.Meth.LevelSubmissionRepository>();
        services.AddTransient<Submissions.Repositories.Meth.SampleSubmissionRepository>();
        
        services.AddTransient<Submissions.Repositories.Rna.ExpSubmissionRepository>();
        services.AddTransient<Submissions.Repositories.Rna.SampleSubmissionRepository>();
        
        services.AddTransient<Submissions.Repositories.RnaSc.ExpSubmissionRepository>();
        services.AddTransient<Submissions.Repositories.RnaSc.SampleSubmissionRepository>();
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

        services.AddTransient<IValidator<SampleForm>, SampleFormValidator>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<SmModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<CnvModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<SvModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<BulkExpModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<EmptyModel>>();

        return services;
    }
}
