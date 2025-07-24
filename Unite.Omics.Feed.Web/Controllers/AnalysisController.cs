using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class AnalysisController<TEntry> : Controller where TEntry : class, new()
{
    protected abstract IValidator<TEntry> EntryModelValidator { get; }
    protected abstract IValidator<ResourceModel> ResourceModelValidator { get; }
    protected abstract IReader<TEntry>[] Readers { get; }


    [HttpGet("{id}")]
    public abstract IActionResult Get(long id);

    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public abstract IActionResult Post([FromBody] AnalysisModel<TEntry> model, [FromQuery] bool review = true);

    [HttpGet("file/formats")]
    public virtual IActionResult GetFileFormats()
    {
        var values = Readers
            .Select(reader => reader.Format)
            .ToArray();

        return Ok(values);
    }

    [HttpPost("file")]
    [RequestSizeLimit(100_000_000)]
    public virtual IActionResult PostFile([FromForm] AnalysisForm<TEntry> form, [FromQuery] bool review = true, [FromQuery] string format = null)
    {
        var model = form.Convert();

        if (!form.ResourcesFile.IsEmpty())
        {
            model.Resources = ParseResources(form.ResourcesFile);
            ValidateItems(model.Resources, ResourceModelValidator, "Resources");
        }

        if (!form.EntriesFile.IsEmpty())
        {
            var reader = GetReader(format);
            if (reader == null)
                return BadRequest($"Unsupported file format: {format}");

            model.Entries = ParseEntries(form.EntriesFile, reader);
            ValidateItems(model.Entries, EntryModelValidator, "Entries");
        }

        return ModelState.IsValid ? Post(model, review) : BadRequest(ModelState);
    }


    protected virtual ResourceModel[] ParseResources(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var streamReader = new StreamReader(stream);

        var tsv = streamReader.ReadToEnd();

        return TsvReader.Read<ResourceModel>(tsv).ToArrayOrNull();
    }

    protected virtual TEntry[] ParseEntries(IFormFile file, IReader<TEntry> reader)
    {
        using var stream = file.OpenReadStream();
        using var streamReader = new StreamReader(stream);

        return reader.Read(streamReader);
    }

    protected virtual void ValidateItems<T>(T[] items, IValidator<T> validator, string prefix)
    {
        for (int i = 0; i < items.Length; i++)
        {
            var result = validator.Validate(items[i]);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError($"{prefix}[{i}].{error.PropertyName}", error.ErrorMessage);
                }
            }
        }
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
