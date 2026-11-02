using Microsoft.AspNetCore.Mvc;

namespace Unite.Omics.Feed.Web.Models.Base;

public record SubmissionForm
{
    [FromForm(Name = "resources")]
    public IFormFile ResourcesFile { get; set; }

    [FromForm(Name = "entries")]
    public IFormFile EntriesFile { get; set; }
}
