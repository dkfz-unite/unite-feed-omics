using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class SubmissionController<TModel, TSubmissionForm>(SubmissionTaskService submissionTaskService, 
    SubmissionRepository<TModel> submissionRepository,
    ILogger logger) : Controller
    where TModel : SubmissionModel, new()
    where TSubmissionForm : SubmissionForm
{
    protected readonly IValidator<ResourceModel> _resourceModelValidator = new ResourceModelValidator();
    
    protected abstract SubmissionTaskType  SubmissionTaskType { get; }
    protected abstract string DataType { get; }

    protected abstract void ValidateModel(TModel model);
    
    protected virtual TModel FindSubmission(string id)
    {
        return submissionRepository.Find(id)?.Document;
    }
    
    protected virtual long AddSubmission(TModel model, bool review)
    {
        string submissionId = submissionRepository.Add(model);
        
        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return submissionTaskService.CreateTask(SubmissionTaskType, submissionId, taskStatus);
    }

    protected virtual void ReadSubmissionForm(TSubmissionForm form, TModel model, string format = null)
    {
        if (!form.ResourcesFile.IsEmpty())
        {
            model.Resources = ParseResources(form.ResourcesFile);
            model.Resources?.ForEach(resource => resource.Type = DataType);
            ValidateResources(model.Resources);
        }
    }
    
    protected virtual ResourceModel[] ParseResources(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var streamReader = new StreamReader(stream);

        var tsv = streamReader.ReadToEnd();

        return TsvReader.Read<ResourceModel>(tsv).ToArrayOrNull();
    }
    
    protected virtual void ValidateResources(ResourceModel[] resources)
    {
        ValidateItems(resources, _resourceModelValidator, "Resources");
    }
    
    protected virtual void ValidateItems<T>(T[] items, IValidator<T> validator, string prefix)
    {
        if (items.IsEmpty())
        {
            ModelState.AddModelError(prefix, "Should not be empty");
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

    protected abstract TModel Convert(TSubmissionForm form);
    
    [HttpGet("{id}")]
    [Authorize]
    public virtual IActionResult Get(long id)
    {
        var task = submissionTaskService.GetTask(id);
        if (task == null)
            return NotFound();

        var submission = FindSubmission(task.Target);

        return Ok(submission);
    }
    
    [HttpGet("{id}/status")]
    [Authorize]
    public virtual IActionResult GetStatus(long id)
    {
        var task = submissionTaskService.GetTask(id);
        if (task == null)
            return NotFound();
        
        return Ok(task.StatusTypeId);
    }
    
    [HttpPost("")]
    [Consumes("application/json")]
    [RequestSizeLimit(100_000_000)]
    [Authorize(Policy = Policies.Data.Writer)]
    public virtual IActionResult PostJson([FromBody] TModel model, [FromQuery] bool review = true)
    {
        model.Resources?.ForEach(resource => resource.Type = DataType);
        ValidateModel(model);

        return ModelState.IsValid ? Ok(AddSubmission(model, review)) : BadRequest(ModelState);
    }
    
    [HttpPost("")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(100_000_000)]
    [Authorize(Policy = Policies.Data.Writer)]
    public IActionResult PostForm([FromForm] TSubmissionForm form, [FromQuery] bool review = true, [FromQuery] string format = null)
    {
        var model = Convert(form);
        ValidateModel(model);

        try
        {
            ReadSubmissionForm(form, model, format);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return ModelState.IsValid ? Ok(AddSubmission(model, review)) : BadRequest(ModelState);
    }
}