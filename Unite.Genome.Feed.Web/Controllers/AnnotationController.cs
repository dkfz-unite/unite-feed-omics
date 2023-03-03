using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Services.Annotation;

namespace Unite.Genome.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
public class AnnotationController : Controller
{
    private readonly MutationAnnotationTaskService _mutationAnnotationTaskService;
    private readonly CopyNumberVariantAnnotationTaskService _copyNumberVariantAnnotationTaskService;
    private readonly StructuralVariantAnnotationTaskService _structuralVariantAnnotationTaskService;


    public AnnotationController(
        MutationAnnotationTaskService mutationAnnotationTaskService,
        CopyNumberVariantAnnotationTaskService copyNumberVariantAnnotationTaskService,
        StructuralVariantAnnotationTaskService structuralVariantAnnotationTaskService)
    {
        _mutationAnnotationTaskService = mutationAnnotationTaskService;
        _copyNumberVariantAnnotationTaskService = copyNumberVariantAnnotationTaskService;
        _structuralVariantAnnotationTaskService = structuralVariantAnnotationTaskService;
    }


    [HttpPost]
    public IActionResult SSM()
    {
        _mutationAnnotationTaskService.CreateTasks();

        return Ok();
    }

    [HttpPost]
    public IActionResult CNV()
    {
        _copyNumberVariantAnnotationTaskService.CreateTasks();

        return Ok();
    }

    [HttpPost]
    public IActionResult SV()
    {
        _structuralVariantAnnotationTaskService.CreateTasks();

        return Ok();
    }
}
