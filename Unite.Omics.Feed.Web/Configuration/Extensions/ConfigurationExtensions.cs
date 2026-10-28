using Unite.Cache.Configuration.Options;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Services.Vep;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Annotation;
using Unite.Omics.Feed.Web.Handlers.Indexing;
using Unite.Omics.Feed.Web.Handlers.Submission;
using Unite.Omics.Feed.Web.Services.Annotation;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;
using Unite.Omics.Feed.Web.Workers;
using Unite.Omics.Indices.Services;


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
        services.AddTransient<CnvpIndexingTaskService>();

        // Submissions hosted services
        services.AddHostedService<SubmissionsWorker>();
        services.AddTransient<RnaSubmissionHandler>();
        services.AddTransient<RnaExpSubmissionHandler>();
        services.AddTransient<RnascSubmissionHandler>();
        services.AddTransient<RnascExpSubmissionHandler>();
        services.AddTransient<ProtExpSubmissionHandler>();
        services.AddTransient<DnaSubmissionHandler>();
        services.AddTransient<DnaSmSubmissionHandler>();
        services.AddTransient<DnaCnvSubmissionHandler>();
        services.AddTransient<DnaSvSubmissionHandler>();
        services.AddTransient<MethSubmissionHandler>();
        services.AddTransient<MethLvlSubmissionHandler>();
        services.AddTransient<DnaCnvpSubmissionHandler>();

        //Indexing Handlers
        services.AddHostedService<IndexingWorker>();
        services.AddTransient<GenesIndexingHandler>();
        services.AddTransient<ProteinsIndexingHandler>();
        services.AddTransient<SmsIndexingHandler>();
        services.AddTransient<CnvsIndexingHandler>();
        services.AddTransient<SvsIndexingHandler>();
        services.AddTransient<CnvpsIndexingHandler>();

        //Index Creators
        services.AddTransient<CnvIndexEntityBuilder>();
        services.AddTransient<SmIndexEntityBuilder>();
        services.AddTransient<SvIndexEntityBuilder>();
        services.AddTransient<GeneIndexEntityBuilder>();
        services.AddTransient<GeneExpressionIndexEntityBuilder>();
        services.AddTransient<ProteinIndexEntityBuilder>();
        services.AddTransient<ProteinExpressionIndexEntityBuilder>();
        services.AddTransient<CnvpIndexEntityBuilder>();

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
        services.AddTransient<CnvpSubmissionRepository>();
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
        services.AddTransient<IGenomeOptions, GenomeOptions>();

        return services;
    }
}
