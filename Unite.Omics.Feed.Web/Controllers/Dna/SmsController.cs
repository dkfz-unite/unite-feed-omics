using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;
using Unite.Data.Helpers.Omics.Dna.Sm;
using Unite.Essentials.Extensions;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Binders;
using Unite.Omics.Feed.Web.Models.Dna.Sm;
using Unite.Omics.Feed.Web.Models.Dna.Sm.Validators;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sm")]
[Authorize(Policy = Policies.Data.Writer)]
public class SmsController : AnalysisDataController<VariantModel>
{
    public SmsController(SubmissionTaskService submissionTaskService,
        ILogger<SmsController> logger,
        SmSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }

    protected override IValidator<VariantModel> EntryModelValidator => new VariantModelValidator();
    protected override string DataType => DataTypes.Omics.Dna.Sm;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];
    protected override string[] Scope =>
    [
        SmType.SNV.ToDefinitionString(),
        SmType.INS.ToDefinitionString(),
        SmType.DEL.ToDefinitionString(),
        SmType.MNV.ToDefinitionString()
    ];
    protected override IReader<VariantModel>[] Readers =>
    [
        new TsvReader<VariantModel>(),
        new Models.Dna.Sm.Readers.Vcf.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_SM;


    protected override void ValidateModel(AnalysisModel<VariantModel> model)
    {
        base.ValidateModel(model);

        ValidateEntryScope(model.Entries, model.TargetSample?.Scope);
    }

    protected override void ValidateEntries(VariantModel[] entries, AnalysisModel<VariantModel> model)
    {
        base.ValidateEntries(entries, model);

        ValidateEntryScope(entries, model.TargetSample?.Scope);
    }


    private void ValidateEntryScope(VariantModel[] entries, string[] scope)
    {
        if (scope.IsEmpty())
            return;

        var types = scope.Select(EnumBinder.Bind<SmType>).ToArray();

        for (var i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];

            if (entry == null || (string.IsNullOrWhiteSpace(entry.Ref) && string.IsNullOrWhiteSpace(entry.Alt)))
                continue;

            var type = TypeDetector.Detect(entry.Ref, entry.Alt);

            if (!types.Contains(type))
                ModelState.AddModelError($"Entries[{i}]", $"Mutation type {type} is outside the declared scope");
        }
    }
}
