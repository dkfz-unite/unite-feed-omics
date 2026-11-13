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
using Unite.Omics.Feed.Web.Models.Dna.Cnv.Validators;
using Unite.Omics.Feed.Web.Models.Dna.Cnv;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;
using Unite.Omics.Annotations.Services.Liftover;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Web.Models.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/cnv")]
[Authorize(Policy = Policies.Data.Writer)]
public class CnvsController : VariantsController<VariantModel>
{
    private readonly CnvsLiftoverService _liftoverService;

    protected override string DataType => DataTypes.Omics.Dna.Cnv;
    protected override IValidator<VariantAnalysisModel<VariantModel>> ModelValidator => new AnalysisDataModelValidator<VariantModel>([AnalysisType.WGS, AnalysisType.WES], new VariantModelValidator());
    protected override IFormConverter<VariantAnalysisModel<VariantModel>, AnalysisForm, VariantModel> FormConverter => new AnalysisFormConverter<VariantModel, VariantAnalysisModel<VariantModel>>();
    protected override IReader<VariantModel>[] EntryReaders =>
    [
        new TsvReader<VariantModel>(),
        new Models.Dna.Cnv.Readers.Aceseq.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_CNV;


    public CnvsController(SubmissionTaskService submissionTaskService,
        ILogger<CnvsController> logger,
        CnvSubmissionRepository submissionRepository,
        IGenomeOptions genomeOptions,
        CnvsLiftoverService liftoverService) : base(submissionTaskService, submissionRepository, genomeOptions, logger)
    {
        _liftoverService = liftoverService;
    }

    protected override async Task<LiftoverResult<VariantModel>[]> Lift(byte sourceGenome, byte targetGenome, VariantModel[] variants)
    {
        var resources = variants.Select(variant => new CnvResource
        {
            Chromosome = variant.Chromosome,
            Start = variant.Start,
            End = variant.End
        }).ToArray();

        var results = await _liftoverService.Lift(sourceGenome, targetGenome, resources);

        return results.Select((result, index) => new LiftoverResult<VariantModel>(
            result.Target == null ? variants[index] : variants[index] with
            {
                Chromosome = result.Target.Chromosome,
                Start = result.Target.Start,
                End = result.Target.End
            },
            result.Reason)).ToArray();
    }

}
