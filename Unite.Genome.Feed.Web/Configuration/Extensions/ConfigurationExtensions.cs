using FluentValidation;
using Unite.Cache.Configuration.Options;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Data.Services.Tasks;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Services.Transcriptomics;
using Unite.Genome.Annotations.Services.Vep;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Annotation;
using Unite.Genome.Feed.Web.Handlers.Indexing;
using Unite.Genome.Feed.Web.Handlers.Submission;
using Unite.Genome.Feed.Web.HostedServices;
using Unite.Genome.Feed.Web.Models.Transcriptomics;
using Unite.Genome.Feed.Web.Models.Transcriptomics.Validators;
using Unite.Genome.Feed.Web.Models.Variants;
using Unite.Genome.Feed.Web.Models.Variants.Validators;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;
using Unite.Genome.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

using VariantEntities = Unite.Data.Entities.Genome.Variants;
using VariantModels = Unite.Genome.Feed.Web.Models.Variants;

namespace Unite.Genome.Feed.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static void Configure(this IServiceCollection services)
    {
        services.AddTransient<ApiOptions>();
        services.AddTransient<ISqlOptions, SqlOptions>();
        services.AddTransient<IMongoOptions, MongoOptions>();
        services.AddTransient<IElasticOptions, ElasticOptions>();
        services.AddTransient<IEnsemblVepOptions, EnsemblVepOptions>();
        services.AddTransient<IEnsemblOptions, EnsemblOptions>();

        services.AddTransient<IValidator<SequencingDataModel<VariantModels.SSM.VariantModel>[]>, SequencingDataModelsValidator<VariantModels.SSM.VariantModel, VariantModels.SSM.Validators.VariantModelValidator>>();
        services.AddTransient<IValidator<SequencingDataModel<VariantModels.CNV.VariantModel>[]>, SequencingDataModelsValidator<VariantModels.CNV.VariantModel, VariantModels.CNV.Validators.VariantModelValidator>>();
        services.AddTransient<IValidator<SequencingDataModel<VariantModels.CNV.VariantAceSeqModel>[]>, SequencingDataModelsValidator<VariantModels.CNV.VariantAceSeqModel, VariantModels.CNV.Validators.VariantAceSeqModelValidator>>();
        services.AddTransient<IValidator<SequencingDataModel<VariantModels.SV.VariantModel>[]>, SequencingDataModelsValidator<VariantModels.SV.VariantModel, VariantModels.SV.Validators.VariantModelValidator>>();
        services.AddTransient<IValidator<TranscriptomicsDataModel>, TranscriptomicsDataModelValidator>();

        services.AddTransient<DomainDbContext>();
        services.AddTransient<SequencingDataWriter>();
        services.AddTransient<TranscriptomicsDataWriter>();

        // Submission services
        services.AddTransient<VariantsSubmissionService>();
        services.AddTransient<TranscriptomicsSubmissionService>();

        // Annotation services
        services.AddTransient<MutationsAnnotationService>();
        services.AddTransient<CopyNumberVariantsAnnotationService>();
        services.AddTransient<StructuralVariantsAnnotationService>();
        services.AddTransient<TranscriptomicsAnnotationService>();

        // Task processing services
        services.AddTransient<TasksProcessingService>();

        // Task creation services
        services.AddTransient<SubmissionTaskService>();
        services.AddTransient<GeneIndexingTaskService>();
        services.AddTransient<MutationAnnotationTaskService>();
        services.AddTransient<MutationIndexingTaskService>();
        services.AddTransient<CopyNumberVariantAnnotationTaskService>();
        services.AddTransient<CopyNumberVariantIndexingTaskService>();
        services.AddTransient<StructuralVariantAnnotationTaskService>();
        services.AddTransient<StructuralVariantIndexingTaskService>();

        // Submissions hosted services
        services.AddHostedService<SubmissionsHostedService>();
        services.AddTransient<TranscriptomicsSubmissionHandler>();
        services.AddTransient<MutationsSubmissionHandler>();
        services.AddTransient<CopyNumberVariantsSubmissionHandler>();
        services.AddTransient<StructuralVariantsSubmissionHandler>();


        // Variants annotation hosted service
        services.AddHostedService<VariantsAnnotationHostedService>();
        services.AddTransient<VariantsAnnotationOptions>();
        services.AddTransient<MutationsAnnotationHandler>();
        services.AddTransient<CopyNumberVariantsAnnotationHandler>();
        services.AddTransient<StructuralVariantsAnnotationHandler>();

        // Variants indexing hosted service
        services.AddHostedService<VariantsIndexingHostedService>();
        services.AddTransient<VariantsIndexingOptions>();
        services.AddTransient<MutationsIndexingHandler>();
        services.AddTransient<CopyNumberVariantsIndexingHandler>();
        services.AddTransient<StructuralVariantsIndexingHandler>();

        // Genes indexing hosted service
        services.AddHostedService<GenesIndexingHostedService>();
        services.AddTransient<GenesIndexingOptions>();
        services.AddTransient<GenesIndexingHandler>();

        // Variant indexing services
        services.AddTransient<VariantIndexCreationService<VariantEntities.SSM.Variant, VariantEntities.SSM.VariantOccurrence>>();
        services.AddTransient<VariantIndexCreationService<VariantEntities.CNV.Variant, VariantEntities.CNV.VariantOccurrence>>();
        services.AddTransient<VariantIndexCreationService<VariantEntities.SV.Variant, VariantEntities.SV.VariantOccurrence>>();
        services.AddTransient<VariantsIndexingService>();

        // Gene indexing services
        services.AddTransient<GeneIndexCreationService>();
        services.AddTransient<GenesIndexingService>();
    }
}
