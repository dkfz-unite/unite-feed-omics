using Unite.Data.Context.Services.Tasks;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

public abstract class VariantsController<TVariant> : SubmissionController<VariantAnalysisModel<TVariant>, AnalysisForm, TVariant>
    where TVariant : class, new()
{
    private readonly IGenomeOptions _genomeOptions;


    protected VariantsController(
        SubmissionTaskService submissionTaskService,
        SubmissionRepository<VariantAnalysisModel<TVariant>> submissionRepository,
        IGenomeOptions genomeOptions,
        ILogger logger) : base(submissionTaskService, submissionRepository, logger)
    {
        _genomeOptions = genomeOptions;
    }

    protected override async Task PrepareSubmission(VariantAnalysisModel<TVariant> model)
    {
        model.LiftoverFailures = null;

        if (string.Equals(model.TargetSample.Genome, _genomeOptions.Build, Comparison))
            return;

        var sourceGenome = GetGenomeVersion(model.TargetSample.Genome);
        var results = await Lift(sourceGenome, _genomeOptions.Version, model.Entries);
        var failures = new List<LiftoverResult<TVariant>>();
        var lifted = new List<TVariant>();

        foreach (var result in results)
        {
            if (string.IsNullOrWhiteSpace(result.Failure))
                lifted.Add(result.Variant);
            else
                failures.Add(result);
        }

        model.Entries = lifted.ToArray();
        model.LiftoverFailures = failures.ToArray();

        if (model.Entries.Length == 0)
            ModelState.AddModelError("Entries", $"No variants could be lifted from {model.TargetSample.Genome} to {_genomeOptions.Build}");
    }

    protected abstract Task<LiftoverResult<TVariant>[]> Lift(byte sourceGenome, byte targetGenome, TVariant[] variants);

    private static byte GetGenomeVersion(string genome)
    {
        return genome.Equals(Genomes.GRCh37, Comparison) ? (byte)37 : (byte)38;
    }
}
