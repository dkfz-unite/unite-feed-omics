using FluentValidation;
using Unite.Data.Context.Services.Tasks;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class AnalysisDataController<TEntry> : AnalysisController<TEntry>
    where TEntry : class, new()
{
    protected AnalysisDataController(SubmissionTaskService submissionTaskService,
        SubmissionRepository<AnalysisModel<TEntry>> submissionRepository,
        ILogger<AnalysisDataController<TEntry>> logger) : base(submissionTaskService, submissionRepository, logger)
    {
    }

    protected abstract IValidator<TEntry> EntryModelValidator { get; }
    protected abstract IReader<TEntry>[] Readers { get; }
    
    protected override void ReadSubmissionForm(AnalysisForm form, AnalysisModel<TEntry> model, string format = null)
    {
        base.ReadSubmissionForm(form, model, format);
        
        if (!form.EntriesFile.IsEmpty())
        {
            var reader = GetReader(format);
            if (reader == null)
                throw new InvalidOperationException($"Unsupported file format: {format}");

            model.Entries = ParseEntries(form.EntriesFile, reader);
            ValidateEntries(model.Entries, model);
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
