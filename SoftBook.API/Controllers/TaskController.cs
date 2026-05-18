//using Microsoft.AspNetCore.Mvc;

//namespace SoftBook.API.Controllers
//{
//    public class TaskController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Classes;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    /// <summary>
    /// API Controller for Task management.
    /// Provides endpoints for CRUD operations on Tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ITenantProvider _tenantProvider;

        public TaskController(ITaskService taskService, ITenantProvider tenantProvider)
        {
            _taskService = taskService;
            _tenantProvider = tenantProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(int companyId)
        {
            var tasks = await _taskService.GetTasks(companyId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int companyId,long id)
        {
            var task = await _taskService.GetTaskById(companyId,id);
            if (task == null)
            {
                return NotFound(new { Message = "Task not found" });
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(int companyId, [FromBody] TaskModel model)
        {
            if (model == null)
            {
                return BadRequest(new { Message = "Invalid task data" });
            }

            long taskId = await _taskService.AddTask(companyId,model);

            return Ok(new
            {
                Message = "Task Added Successfully",
                TaskId = taskId
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask(int companyId, [FromBody] TaskModel model, [FromQuery] string updateType = "this")
        {
            if (model == null || model.TaskId <= 0)
            {
                return BadRequest(new { Message = "Invalid task data" });
            }

            bool success = await _taskService.UpdateTask(companyId, model, updateType);
            if (success)
            {
                return Ok(new { Message = "Task Updated Successfully" });
            }
            return NotFound(new { Message = "Task not found or update failed" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int companyId, long id, [FromQuery] string deleteType = "this")
        {
            bool success = await _taskService.DeleteTask(companyId, id, deleteType);
            if (success)
            {
                return Ok(new { Message = "Task Deleted Successfully" });
            }
            return NotFound(new { Message = "Task not found or already deleted" });
        }
    }
}