using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sv.Validators;
using Unite.Omics.Feed.Web.Models.Dna.Sv;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;
using Unite.Omics.Annotations.Services.Liftover;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Web.Models.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sv")]
[Authorize(Policy = Policies.Data.Writer)]
public class SvsController : VariantsController<VariantModel>
{
    private readonly SvsLiftoverService _liftoverService;

    protected override string DataType => DataTypes.Omics.Dna.Sv;
    protected override IValidator<VariantAnalysisModel<VariantModel>> ModelValidator => new AnalysisDataModelValidator<VariantModel>([AnalysisType.WGS, AnalysisType.WES], new VariantModelValidator());
    protected override IFormConverter<VariantAnalysisModel<VariantModel>, AnalysisForm, VariantModel> FormConverter => new AnalysisFormConverter<VariantModel, VariantAnalysisModel<VariantModel>>();
    protected override IReader<VariantModel>[] EntryReaders =>
    [
        new TsvReader<VariantModel>(),
        new Models.Dna.Sv.Readers.DkfzSophia.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_SV;


    public SvsController(SubmissionTaskService submissionTaskService,
        ILogger<SvsController> logger,
        SvSubmissionRepository submissionRepository,
        IGenomeOptions genomeOptions,
        SvsLiftoverService liftoverService) : base(submissionTaskService, submissionRepository, genomeOptions, logger)
    {
        _liftoverService = liftoverService;
    }

    protected override async Task<LiftoverResult<VariantModel>[]> Lift(byte sourceGenome, byte targetGenome, VariantModel[] variants)
    {
        var resources = variants.Select(variant => new SvResource
        {
            Chromosome = variant.Chromosome,
            Start = variant.Start,
            End = variant.End,
            OtherChromosome = variant.OtherChromosome,
            OtherStart = variant.OtherStart,
            OtherEnd = variant.OtherEnd,
            Type = variant.Type,
            Inverted = variant.Inverted
        }).ToArray();

        var results = await _liftoverService.Lift(sourceGenome, targetGenome, resources);

        return results.Select((result, index) => new LiftoverResult<VariantModel>(
            result.Target == null ? variants[index] : variants[index] with
            {
                Chromosome = result.Target.Chromosome,
                Start = result.Target.Start,
                End = result.Target.End,
                OtherChromosome = result.Target.OtherChromosome,
                OtherStart = result.Target.OtherStart,
                OtherEnd = result.Target.OtherEnd,
                Type = result.Target.Type,
                Inverted = result.Target.Inverted
            },
            result.Reason)).ToArray();
    }
}
