using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class AnalysisDataController<TEntry> : Controller where TEntry : class, new()
{
    protected abstract IValidator<TEntry> EntryModelValidator { get; }
    protected abstract IValidator<ResourceModel> ResourceModelValidator { get; }
    protected abstract string DataType { get; }
    protected abstract AnalysisType[] AnalysisTypes { get; }
    protected abstract IReader<TEntry>[] Readers { get; }


    [HttpGet("{id}")]
    public virtual IActionResult Get(long id)
    {
        var submission = GetSubmission(id);

        return Ok(submission);
    }

    [HttpPost("")]
    [Consumes("application/json")]
    [RequestSizeLimit(100_000_000)]
    public virtual IActionResult PostJson([FromBody] AnalysisModel<TEntry> model, [FromQuery] bool review = true)
    {
        model.Resources?.ForEach(resource => resource.Type = DataType);
        ValidateAnalysis(model);

        return ModelState.IsValid ? Ok(AddSubmission(model, review)) : BadRequest(ModelState);
    }
    
    [HttpPost("")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(100_000_000)]
    public virtual IActionResult PostForm([FromForm] AnalysisForm<TEntry> form, [FromQuery] bool review = true, [FromQuery] string format = null)
    {
        var model = form.Convert();
        ValidateAnalysis(model);

        if (!form.ResourcesFile.IsEmpty())
        {
            model.Resources = ParseResources(form.ResourcesFile);
            model.Resources?.ForEach(resource => resource.Type = DataType);
            ValidateResources(model.Resources, model);
        }

        if (!form.EntriesFile.IsEmpty())
        {
            var reader = GetReader(format);
            if (reader == null)
                return BadRequest($"Unsupported file format: {format}");

            model.Entries = ParseEntries(form.EntriesFile, reader);
            ValidateEntries(model.Entries, model);
        }

        return ModelState.IsValid ? Ok(AddSubmission(model, review)) : BadRequest(ModelState);
    }


    protected abstract AnalysisModel<TEntry> GetSubmission(long id);

    protected abstract long AddSubmission(AnalysisModel<TEntry> model, bool review);


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

    protected virtual void ValidateAnalysis(AnalysisModel<TEntry> model)
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

    protected virtual void ValidateResources(ResourceModel[] resources, AnalysisModel<TEntry> model)
    {
        ValidateItems(resources, ResourceModelValidator, "Resources");
    }

    protected virtual void ValidateEntries(TEntry[] entries, AnalysisModel<TEntry> model)
    {
        ValidateItems(entries, EntryModelValidator, "Entries");
    }

    protected void ValidateItems<T>(T[] items, IValidator<T> validator, string prefix)
    {
        if (items.IsEmpty())
        {
            ModelState.AddModelError(prefix, "No records found.");
            return;
        }

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
