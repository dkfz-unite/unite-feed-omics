using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class SubmissionController<TModel, TForm, TEntry> : Controller
    where TModel : SubmissionModel, new()
    where TForm : SubmissionForm
    where TEntry : class, new()
{
    protected const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;


    protected readonly SubmissionTaskService _submissionTaskService;
    protected readonly SubmissionRepository<TModel> _submissionRepository;


    protected abstract SubmissionTaskType SubmissionTaskType { get; }
    protected abstract string DataType { get; }
    protected abstract IValidator<TModel> ModelValidator { get; }
    protected virtual IReader<TEntry>[] EntryReaders => [];
    protected virtual IReader<ResourceModel> ResourceReader => new TsvReader<ResourceModel>();
    protected abstract IFormConverter<TModel, TForm, TEntry> FormConverter { get; }


    protected SubmissionController(SubmissionTaskService submissionTaskService,
        SubmissionRepository<TModel> submissionRepository, ILogger logger)
    {
        _submissionTaskService = submissionTaskService;
        _submissionRepository = submissionRepository;
    }


    [HttpGet("{id}")]
    [Authorize]
    public virtual IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);
        return task == null ? NotFound() : Ok(FindSubmission(task.Target));
    }

    [HttpGet("{id}/status")]
    [Authorize]
    public virtual IActionResult GetStatus(long id)
    {
        var task = _submissionTaskService.GetTask(id);
        return task == null ? NotFound() : Ok(task.StatusTypeId);
    }

    [HttpPost]
    [Consumes("application/json")]
    [RequestSizeLimit(100_000_000)]
    [Authorize(Policy = Policies.Data.Writer)]
    public IActionResult PostJson([FromBody] TModel model, [FromQuery] bool review = true)
    {
        return Submit(model, review);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(100_000_000)]
    [Authorize(Policy = Policies.Data.Writer)]
    public IActionResult PostForm([FromForm] TForm form, [FromQuery] bool review = true, [FromQuery] string format = null)
    {
        var resources = ReadFile(form.ResourcesFile, ResourceReader, "Resources");
        TEntry[] entries = null;
        var readers = EntryReaders;

        if (form.EntriesFile != null && readers.Length > 0)
        {
            var reader = string.IsNullOrWhiteSpace(format)
                ? readers[0]
                : readers.FirstOrDefault(reader => reader.Format.Equals(format.Trim(), Comparison));

            if (reader == null)
                ModelState.AddModelError("Format", $"Unsupported entries format: {format}");
            else
                entries = ReadFile(form.EntriesFile, reader, "Entries");
        }

        var model = FormConverter.Convert(form, entries, resources);
        return Submit(model, review);
    }


    protected virtual TModel FindSubmission(string id)
    {
        return _submissionRepository.Find(id)?.Document;
    }

    protected virtual long AddSubmission(TModel model, bool review)
    {
        var submissionId = _submissionRepository.Add(model);
        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;
        return _submissionTaskService.CreateTask(SubmissionTaskType, submissionId, taskStatus);
    }


    private IActionResult Submit(TModel model, bool review)
    {
        model ??= new TModel();
        var validation = ModelValidator.Validate(model);
        foreach (var error in validation.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        foreach (var resource in model.Resources ?? [])
            resource.Type = DataType;

        return Ok(AddSubmission(model, review));
    }

    private T[] ReadFile<T>(IFormFile file, IReader<T> reader, string property) where T : class, new()
    {
        if (file == null)
            return null;

        if (reader == null)
            throw new InvalidOperationException("No file reader configured");

        using var stream = file.OpenReadStream();
        using var streamReader = new StreamReader(stream);

        if (TryReadFile(streamReader, reader, out var values))
            return values;

        ModelState.AddModelError(property, $"File could not be read in '{reader.Format}' format");
        return null;
    }

    private static bool TryReadFile<T>(StreamReader stream, IReader<T> reader, out T[] values) where T : class, new()
    {
        values = null;
        try
        {
            values = reader.Read(stream);
            return values != null;
        }
        catch
        {
            return false;
        }
    }
}
