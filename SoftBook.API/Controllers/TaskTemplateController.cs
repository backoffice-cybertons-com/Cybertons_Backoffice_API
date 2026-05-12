using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskTemplateController : ControllerBase
    {
        private readonly ITaskTemplateService _templateService;

        public TaskTemplateController(ITaskTemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates(int companyId)
        {
            var templates = await _templateService.GetTemplates(companyId);
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplateById(int companyId, long id)
        {
            var template = await _templateService.GetTemplateById(companyId,id);
            if (template == null)
            {
                return NotFound(new { Message = "Template not found" });
            }
            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> AddTemplate(int companyId, [FromBody] TaskTemplateModel model)
        {
            if (model == null)
            {
                return BadRequest(new { Message = "Invalid template data" });
            }

            long templateId = await _templateService.AddTemplate(companyId,model);

            return Ok(new
            {
                Message = "Template Created Successfully",
                TemplateId = templateId
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTemplate(int companyId, [FromBody] TaskTemplateModel model)
        {
            if (model == null || model.TemplateId <= 0)
            {
                return BadRequest(new { Message = "Invalid template data" });
            }

            bool success = await _templateService.UpdateTemplate(companyId, model);
            if (success)
            {
                return Ok(new { Message = "Template Updated Successfully" });
            }
            return NotFound(new { Message = "Template not found or update failed" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int companyId, long id)
        {
            bool success = await _templateService.DeleteTemplate(companyId, id);
            if (success)
            {
                return Ok(new { Message = "Template Deleted Successfully" });
            }
            return NotFound(new { Message = "Template not found or already deleted" });
        }
    }
}
