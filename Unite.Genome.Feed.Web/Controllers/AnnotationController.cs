using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Services;

namespace Unite.Genome.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
public class AnnotationController : Controller
{
    private readonly MutationAnnotationTaskService _annotationTaskService;


    public AnnotationController(MutationAnnotationTaskService annotationTaskService)
    {
        _annotationTaskService = annotationTaskService;
    }


    [HttpPost]
    public IActionResult Mutations()
    {
        _annotationTaskService.CreateTasks();

        return Ok();
    }
}
