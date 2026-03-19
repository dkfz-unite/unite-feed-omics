using Microsoft.AspNetCore.Mvc;

namespace Unite.Omics.Feed.Web.Models.Base;

public record SubmissionForm
{
    protected IFormFile _resourcesFile;
    
    /// <summary>
    /// Tsv file with resources metadata.
    /// </summary>
    [FromForm(Name = "resources")]
    public IFormFile ResourcesFile { get => _resourcesFile; init => _resourcesFile = value; }
}