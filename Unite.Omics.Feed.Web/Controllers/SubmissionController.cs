using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers;

public abstract class SubmissionController<TModel>(SubmissionTaskService submissionTaskService) : Controller
    where TModel : SubmissionModel, new()
{
    protected abstract SubmissionTaskType  SubmissionTaskType { get; }
    protected abstract SubmissionRepository SubmissionRepository { get; }
    protected abstract string DataType { get; }

    protected abstract void ValidateModel(TModel model);
    
    protected virtual TModel FindSubmission(string id)
    {
        return SubmissionRepository.Find<TModel>(id)?.Document;
    }
    
    protected virtual long AddSubmission(TModel model, bool review)
    {
        string submissionId = SubmissionRepository.Add(model);
        
        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return submissionTaskService.CreateTask(SubmissionTaskType, submissionId, taskStatus);
    }
    
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
}