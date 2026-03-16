using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Cache.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Vep;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers;
using Unite.Omics.Feed.Web.Handlers.Annotation;
using Unite.Omics.Feed.Web.Handlers.Indexing;
using Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;
using Unite.Omics.Feed.Web.Handlers.Submission;
using Unite.Omics.Feed.Web.Workers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Services.Annotation;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;
using Unite.Omics.Indices.Services;

using SmModel = Unite.Omics.Feed.Web.Models.Dna.Sm.VariantModel;
using SmModelValidator = Unite.Omics.Feed.Web.Models.Dna.Sm.Validators.VariantModelValidator;
using CnvModel = Unite.Omics.Feed.Web.Models.Dna.Cnv.VariantModel;
using CnvModelValidator = Unite.Omics.Feed.Web.Models.Dna.Cnv.Validators.VariantModelValidator;
using SvModel = Unite.Omics.Feed.Web.Models.Dna.Sv.VariantModel;
using SvModelValidator = Unite.Omics.Feed.Web.Models.Dna.Sv.Validators.VariantModelValidator;
using GeneExpModel = Unite.Omics.Feed.Web.Models.Rna.ExpressionModel;
using GeneExpModelValidator = Unite.Omics.Feed.Web.Models.Rna.Validators.ExpressionModelValidator;
using ProtExpModel = Unite.Omics.Feed.Web.Models.Prot.ExpressionModel;
using ProtExpModelValidator = Unite.Omics.Feed.Web.Models.Prot.Validators.ExpressionModelValidator;


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
        services.AddTransient<Data.Writers.Prot.AnalysisWriter>();

        // Annotation services
        services.AddTransient<SmsAnnotationService>();
        services.AddTransient<CnvsAnnotationService>();
        services.AddTransient<SvsAnnotationService>();
        services.AddTransient<Annotations.Services.Rna.ExpressionsAnnotationService>();
        services.AddTransient<Annotations.Services.Prot.ExpressionsAnnotationService>();

        // Task processing services
        services.AddTransient<TasksProcessingService>();

        // Task creation services
        services.AddTransient<SubmissionTaskService>();
        services.AddTransient<SampleIndexingTaskService>();
        services.AddTransient<GeneIndexingTaskService>();
        services.AddTransient<ProteinIndexingTaskService>();
        services.AddTransient<SmAnnotationTaskService>();
        services.AddTransient<SmIndexingTaskService>();
        services.AddTransient<CnvAnnotationTaskService>();
        services.AddTransient<CnvIndexingTaskService>();
        services.AddTransient<SvAnnotationTaskService>();
        services.AddTransient<SvIndexingTaskService>();
        services.AddTransient<CnvProfileIndexingTaskService>();

        // Submissions hosted services
        services.AddHostedService<SubmissionsWorker>();
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<RnaSubmissionHandler>(sp, HandlerPriority.Highest));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<RnaExpSubmissionHandler>(sp, HandlerPriority.Highest));
        services.AddTransient<ISubmissionHandler>(sp => ActivatorUtilities.CreateInstance<ProtExpSubmissionHandler>(sp, HandlerPriority.Highest));
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
        services.AddTransient<IIndexingHandler, ProteinsIndexingHandler>();
        
        //Indexers
        services.AddTransient<CnvIndexer>();
        services.AddTransient<CnvProfileIndexer>();
        services.AddTransient<GeneExpressionIndexer>();
        services.AddTransient<GeneIndexer>();
        services.AddTransient<ProteinExpressionIndexer>();
        services.AddTransient<ProteinIndexer>();
        services.AddTransient<SmIndexer>();
        services.AddTransient<SvIndexer>();
        
        //Index Creators
        services.AddTransient<CnvIndexEntityBuilder>();
        services.AddTransient<SmIndexEntityBuilder>();
        services.AddTransient<SvIndexEntityBuilder>();
        services.AddTransient<GeneIndexEntityBuilder>();
        services.AddTransient<GeneExpressionIndexEntityBuilder>();
        services.AddTransient<ProteinIndexEntityBuilder>();
        services.AddTransient<ProteinExpressionIndexEntityBuilder>();
        services.AddTransient<CnvProfileIndexEntityBuilder>();

        // Variants annotation hosted service
        services.AddHostedService<VariantsAnnotationWorker>();
        services.AddTransient<VariantsAnnotationOptions>();
        services.AddTransient<SmsAnnotationHandler>();
        services.AddTransient<CnvsAnnotationHandler>();
        services.AddTransient<SvsAnnotationHandler>();

        // Variants indexing hosted service
        services.AddTransient<VariantsIndexingOptions>();

        // Genes indexing hosted service
        services.AddTransient<GenesIndexingOptions>();
        
        // Genes indexing hosted service
        services.AddTransient<ProteinsIndexingOptions>();
        
        //Submission repositories
        services.AddTransient<SampleSubmissionRepository>();
        services.AddTransient<CnvProfileSubmissionRepository>();
        services.AddTransient<CnvSubmissionRepository>();
        services.AddTransient<SmSubmissionRepository>();
        services.AddTransient<SvSubmissionRepository>();
        
        services.AddTransient<Submissions.Repositories.Meth.LevelSubmissionRepository>();
        services.AddTransient<Submissions.Repositories.Meth.SampleSubmissionRepository>();
        
        services.AddTransient<Submissions.Repositories.Rna.ExpressionSubmissionRepository>();
        services.AddTransient<Submissions.Repositories.Rna.SampleSubmissionRepository>();
        
        services.AddTransient<Submissions.Repositories.RnaSc.ExpressionSubmissionRepository>();
        services.AddTransient<Submissions.Repositories.RnaSc.SampleSubmissionRepository>();

        services.AddTransient<Submissions.Repositories.Prot.ExpressionSubmissionRepository>();
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
        services.AddTransient<IValidator<AnalysisModel<GeneExpModel>>, AnalysisModelValidator<GeneExpModel, GeneExpModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<EmptyModel>>, AnalysisModelValidator<EmptyModel, EmptyModelValidator>>();
        services.AddTransient<IValidator<AnalysisModel<ProtExpModel>>, AnalysisModelValidator<ProtExpModel, ProtExpModelValidator>>();

        services.AddTransient<IValidator<SampleForm>, SampleFormValidator>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<SmModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<CnvModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<SvModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<GeneExpModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<ProtExpModel>>();
        services.AddTransient<IValidator<AnalysisForm>, AnalysisFormValidator<EmptyModel>>();

        return services;
    }
}
