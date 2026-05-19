using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftBook.Application.Services.Classes;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Infrastructure.Classes;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SoftBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KanbanController : ControllerBase
    {
        private readonly IKanbanService _kanbanService;
        private readonly ITenantProvider _tenantProvider;
        private readonly IWebHostEnvironment _environment;

        public KanbanController(ITenantProvider tenantProvider, IConfiguration configuration, IServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            _tenantProvider = tenantProvider;
            _environment = environment;

            // Self-wire DI dependencies to run flawlessly without modifying Program.cs
            var service = serviceProvider.GetService<IKanbanService>();
            if (service == null)
            {
                var repo = new KanbanRepository(configuration, tenantProvider);
                _kanbanService = new KanbanService(repo);
            }
            else
            {
                _kanbanService = service;
            }
        }

        [HttpPost("upload-cover")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCover(IFormFile file)
        {
            const long maxFileSize = 2 * 1024 * 1024;
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            string[] allowedContentTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Cover image file is required." });
            }

            if (file.Length > maxFileSize)
            {
                return BadRequest(new { Message = "Cover image must be 2MB or smaller." });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType?.ToLowerInvariant() ?? "";
            if (!allowedExtensions.Contains(extension) || !allowedContentTypes.Contains(contentType))
            {
                return BadRequest(new { Message = "Only JPG, PNG, WEBP, and GIF images are allowed." });
            }

            try
            {
                var webRootPath = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                var uploadPath = Path.Combine(webRootPath, "uploads", "kanban", "covers");
                Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await file.CopyToAsync(stream);
                }

                var relativeUrl = $"/uploads/kanban/covers/{fileName}";
                return Ok(new { Url = relativeUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Unable to upload cover image. {ex.Message}" });
            }
        }

        [HttpPost("upload-attachment")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAttachment(IFormFile file)
        {
            const long maxFileSize = 20 * 1024 * 1024; // 20MB
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv", ".zip", ".rar" };

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Attachment file is required." });
            }

            if (file.Length > maxFileSize)
            {
                return BadRequest(new { Message = "Attachment file must be 20MB or smaller." });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { Message = "File extension is not allowed." });
            }

            try
            {
                var webRootPath = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                var uploadPath = Path.Combine(webRootPath, "uploads", "kanban", "attachments");
                Directory.CreateDirectory(uploadPath);

                var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await file.CopyToAsync(stream);
                }

                var relativeUrl = $"/uploads/kanban/attachments/{uniqueFileName}";
                return Ok(new { Url = relativeUrl, Name = file.FileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Unable to upload file. {ex.Message}" });
            }
        }

        private int GetCompanyId(int? companyId)
        {
            return companyId ?? _tenantProvider.CompanyId ?? 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetColumns([FromQuery] int? companyId)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0)
            {
                return BadRequest(new { Message = "Company context not found." });
            }

            try
            {
                var columns = await _kanbanService.GetColumnsAsync(cid);
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromQuery] int? companyId, [FromBody] System.Text.Json.JsonElement payload)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0)
            {
                return BadRequest(new { Message = "Company context not found." });
            }

            try
            {
                // Determine if adding Column or Task/Card:
                // Column payloads do not have 'kanId', whereas Task/Card payloads do.
                if (payload.TryGetProperty("kanId", out var kanIdProp) && kanIdProp.ValueKind != System.Text.Json.JsonValueKind.Null && kanIdProp.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                {
                    // It is a Task/Card!
                    var task = System.Text.Json.JsonSerializer.Deserialize<KanbanTaskModel>(payload.GetRawText());
                    if (task == null) return BadRequest(new { Message = "Invalid task data." });

                    await _kanbanService.SaveTaskAsync(cid, task);
                    return Ok(task);
                }
                else
                {
                    // It is a Column!
                    var col = System.Text.Json.JsonSerializer.Deserialize<KanbanColumnModel>(payload.GetRawText());
                    if (col == null) return BadRequest(new { Message = "Invalid column data." });

                    await _kanbanService.SaveColumnAsync(cid, col);
                    return Ok(col);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromQuery] int? companyId, [FromBody] KanbanTaskModel task)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0)
            {
                return BadRequest(new { Message = "Company context not found." });
            }

            if (task == null || string.IsNullOrEmpty(task.Id))
            {
                return BadRequest(new { Message = "Invalid task data." });
            }

            try
            {
                await _kanbanService.SaveTaskAsync(cid, task);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int? companyId, [FromHeader] string card)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0)
            {
                return BadRequest(new { Message = "Company context not found." });
            }

            if (string.IsNullOrEmpty(card))
            {
                return BadRequest(new { Message = "Invalid card ID." });
            }

            try
            {
                await _kanbanService.DeleteTaskAsync(cid, card);
                return Ok(card);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("column/{columnId}")]
        public async Task<IActionResult> DeleteColumn(string columnId, [FromQuery] int? companyId)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0)
            {
                return BadRequest(new { Message = "Company context not found." });
            }

            if (string.IsNullOrEmpty(columnId))
            {
                return BadRequest(new { Message = "Invalid column ID." });
            }

            try
            {
                var result = await _kanbanService.DeleteColumnAsync(cid, columnId);
                return Ok(new { Success = result, ColumnId = columnId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet("checklist")]
        public async Task<IActionResult> GetChecklistItems([FromQuery] long taskId)
        {
            try
            {
                var items = await _kanbanService.GetChecklistItemsAsync(taskId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("checklist")]
        public async Task<IActionResult> SaveChecklistItem([FromBody] KanbanChecklistItemModel item)
        {
            if (item == null || item.KanbanTaskId <= 0)
            {
                return BadRequest(new { Message = "Invalid checklist item data." });
            }

            try
            {
                if (item.ChecklistItemId <= 0)
                {
                    item.CreatedBy = _tenantProvider.UserId;
                }
                else
                {
                    item.UpdatedBy = _tenantProvider.UserId;
                }

                var savedItem = await _kanbanService.SaveChecklistItemAsync(item);
                return Ok(savedItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("checklist/{itemId}")]
        public async Task<IActionResult> DeleteChecklistItem(long itemId)
        {
            try
            {
                var result = await _kanbanService.DeleteChecklistItemAsync(itemId);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("checklist/group")]
        public async Task<IActionResult> DeleteChecklistGroup([FromQuery] long taskId, [FromQuery] string groupName)
        {
            if (taskId <= 0 || string.IsNullOrEmpty(groupName))
            {
                return BadRequest(new { Message = "Invalid parameters." });
            }

            try
            {
                var result = await _kanbanService.DeleteChecklistGroupAsync(taskId, groupName);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
