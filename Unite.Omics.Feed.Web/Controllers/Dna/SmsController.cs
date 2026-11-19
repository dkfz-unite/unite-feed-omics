using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sm;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;
using Unite.Omics.Annotations.Services.Liftover;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Data.Helpers.Omics.Dna.Sm;
using Unite.Omics.Feed.Web.Models.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sm")]
[Authorize(Policy = Policies.Data.Writer)]
public class SmsController : VariantsController<VariantModel>
{
    private readonly SmsLiftoverService _liftoverService;

    protected override string DataType => DataTypes.Omics.Dna.Sm;
    protected override IValidator<VariantAnalysisModel<VariantModel>> ModelValidator => new Models.Dna.Sm.Validators.AnalysisModelValidator();
    protected override IFormConverter<VariantAnalysisModel<VariantModel>, AnalysisForm, VariantModel> FormConverter => new AnalysisFormConverter<VariantModel, VariantAnalysisModel<VariantModel>>();
    protected override IReader<VariantModel>[] EntryReaders =>
    [
        new TsvReader<VariantModel>(),
        new Models.Dna.Sm.Readers.Vcf.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_SM;


    public SmsController(SubmissionTaskService submissionTaskService,
        ILogger<SmsController> logger,
        SmSubmissionRepository submissionRepository,
        IGenomeOptions genomeOptions,
        SmsLiftoverService liftoverService) : base(submissionTaskService, submissionRepository, genomeOptions, logger)
    {
        _liftoverService = liftoverService;
    }

    protected override async Task<LiftoverResult<VariantModel>[]> Lift(byte sourceGenome, byte targetGenome, VariantModel[] variants)
    {
        var resources = variants.Select(variant =>
        {
            var position = PositionParser.Parse(variant.Position);

            return new SmResource
            {
                Chromosome = variant.Chromosome,
                Start = position.Start,
                End = position.End == position.Start ? null : position.End,
                Ref = variant.Ref,
                Alt = variant.Alt
            };
        }).ToArray();

        var results = await _liftoverService.Lift(sourceGenome, targetGenome, resources);

        return results.Select((result, index) => new LiftoverResult<VariantModel>(
            result.Target == null ? variants[index] : new VariantModel
            {
                Chromosome = result.Target.Chromosome,
                Position = result.Target.End.HasValue
                    ? $"{result.Target.Start}-{result.Target.End}"
                    : result.Target.Start.ToString(),
                Ref = result.Target.Ref,
                Alt = result.Target.Alt
            },
            result.Reason)).ToArray();
    }

}
