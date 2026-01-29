using FluentValidation;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class AnalysisDataController<TEntry>(
    SubmissionTaskService submissionTaskService,
    ILogger<AnalysisDataController<TEntry>> logger)
    : SubmissionController<AnalysisModel<TEntry>, AnalysisForm>(submissionTaskService, logger)
    where TEntry : class, new()
{
    protected abstract IValidator<TEntry> EntryModelValidator { get; }
    protected abstract AnalysisType[] AnalysisTypes { get; }
    protected abstract IReader<TEntry>[] Readers { get; }

    protected override AnalysisModel<TEntry> Convert(AnalysisForm form)
    {
        return AnalysisFormConverter<TEntry>.Convert(form);
    }
    
    protected override void ReadSubmissionForm(AnalysisForm form, AnalysisModel<TEntry> model, string format = null)
    {
        base.ReadSubmissionForm(form, model);
        
        if (!form.EntriesFile.IsEmpty())
        {
            var reader = GetReader(format);
            if (reader == null)
                throw new InvalidOperationException($"Unsupported file format: {format}");

            model.Entries = ParseEntries(form.EntriesFile, reader);
            ValidateEntries(model.Entries, model);
        }
    }

    protected override void ValidateModel(AnalysisModel<TEntry> model)
    {
        if (model.TargetSample != null && !AnalysisTypes.Contains(model.TargetSample.AnalysisType.Value))
        {
            var allowedValues = string.Join(", ", AnalysisTypes.Select(analysis => analysis.ToDefinitionString()));
            ModelState.AddModelError("TargetSample.AnalysisType", $"Allowed values are: {allowedValues}");
        }

        if (model.MatchedSample != null && !AnalysisTypes.Contains(model.MatchedSample.AnalysisType.Value))
        {
            var allowedValues = string.Join(", ", AnalysisTypes.Select(analysis => analysis.ToDefinitionString()));
            ModelState.AddModelError("MatchedSample.AnalysisType", $"Allowed values are [{allowedValues}]");
        }
    }

    protected virtual TEntry[] ParseEntries(IFormFile file, IReader<TEntry> reader)
    {
        using var stream = file.OpenReadStream();
        using var streamReader = new StreamReader(stream);

        return reader.Read(streamReader);
    }

    protected virtual void ValidateEntries(TEntry[] entries, AnalysisModel<TEntry> model)
    {
        ValidateItems(entries, EntryModelValidator, "Entries");
    }

    private IReader<TEntry> GetReader(string format)
    {
        var comparison = StringComparison.InvariantCultureIgnoreCase;

        if (Readers.IsEmpty())
            return null;

        return string.IsNullOrEmpty(format)
            ? Readers.FirstOrDefault()
            : Readers.FirstOrDefault(reader => reader.Format.Equals(format.Trim(), comparison));
    }
}
