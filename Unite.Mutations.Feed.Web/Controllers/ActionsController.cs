using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Mutations.Feed.Data.Services;
using Unite.Mutations.Feed.Data.Services.Mutations;

namespace Unite.Mutations.Feed.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ActionsController : Controller
    {
        private readonly MutationIndexingTaskService _indexingTaskService;
        private readonly VepAnnotationTaskService _annotationTaskService;
        private readonly ILogger _logger;

        public ActionsController(
            MutationIndexingTaskService indexingTaskService,
            VepAnnotationTaskService annotationTaskService,
            ILogger<ActionsController> logger)
        {
            _indexingTaskService = indexingTaskService;
            _annotationTaskService = annotationTaskService;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Index()
        {
            try
            {
                _indexingTaskService.PopulateTasks();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                if(ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException.Message);
                }

                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult Annotate()
        {
            try
            {
                _annotationTaskService.PopulateTasks();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException.Message);
                }

                return BadRequest();
            }
        }
    }
}
