using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Services.Annotation;

namespace Unite.Genome.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
[Authorize(Policy = Policies.Data.Writer)]
public class AnnotationController : Controller
{
    private readonly SsmAnnotationTaskService _ssmAnnotationTaskService;
    private readonly CnvAnnotationTaskService _cnvAnnotationTaskService;
    private readonly SvAnnotationTaskService _svAnnotationTaskService;


    public AnnotationController(
        SsmAnnotationTaskService ssmAnnotationTaskService,
        CnvAnnotationTaskService cnvAnnotationTaskService,
        SvAnnotationTaskService svAnnotationTaskService)
    {
        _ssmAnnotationTaskService = ssmAnnotationTaskService;
        _cnvAnnotationTaskService = cnvAnnotationTaskService;
        _svAnnotationTaskService = svAnnotationTaskService;
    }


    [HttpPost]
    public IActionResult SSM()
    {
        _ssmAnnotationTaskService.CreateTasks();

        return Ok();
    }

    [HttpPost]
    public IActionResult CNV()
    {
        _cnvAnnotationTaskService.CreateTasks();

        return Ok();
    }

    [HttpPost]
    public IActionResult SV()
    {
        _svAnnotationTaskService.CreateTasks();

        return Ok();
    }
}
