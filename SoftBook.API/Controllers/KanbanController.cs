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

        // ─── File uploads ───────────────────────────────────────────────────────────

        [HttpPost("upload-cover")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCover(IFormFile file)
        {
            const long maxFileSize = 2 * 1024 * 1024;
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            string[] allowedContentTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };

            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "Cover image file is required." });

            if (file.Length > maxFileSize)
                return BadRequest(new { Message = "Cover image must be 2MB or smaller." });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType?.ToLowerInvariant() ?? "";
            if (!allowedExtensions.Contains(extension) || !allowedContentTypes.Contains(contentType))
                return BadRequest(new { Message = "Only JPG, PNG, WEBP, and GIF images are allowed." });

            try
            {
                var webRootPath = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootPath))
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var uploadPath = Path.Combine(webRootPath, "uploads", "kanban", "covers");
                Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                    await file.CopyToAsync(stream);

                return Ok(new { Url = $"/uploads/kanban/covers/{fileName}" });
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
            const long maxFileSize = 20 * 1024 * 1024;
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv", ".zip", ".rar" };

            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "Attachment file is required." });

            if (file.Length > maxFileSize)
                return BadRequest(new { Message = "Attachment file must be 20MB or smaller." });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { Message = "File extension is not allowed." });

            try
            {
                var webRootPath = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootPath))
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var uploadPath = Path.Combine(webRootPath, "uploads", "kanban", "attachments");
                Directory.CreateDirectory(uploadPath);

                var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                    await file.CopyToAsync(stream);

                return Ok(new { Url = $"/uploads/kanban/attachments/{uniqueFileName}", Name = file.FileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Unable to upload file. {ex.Message}" });
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────────

        private int GetCompanyId(int? companyId) => companyId ?? _tenantProvider.CompanyId ?? 0;

        // ─── Boards ──────────────────────────────────────────────────────────────────

        /// <summary>Returns all boards owned by the logged-in user for the given company.</summary>
        [HttpGet("boards")]
        public async Task<IActionResult> GetBoards([FromQuery] int? companyId)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0) return BadRequest(new { Message = "Company context not found." });

            try
            {
                var boards = await _kanbanService.GetBoardsAsync(cid);
                return Ok(boards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>Creates a new private board for the logged-in user.</summary>
        [HttpPost("boards")]
        public async Task<IActionResult> CreateBoard([FromQuery] int? companyId, [FromBody] KanbanBoardModel payload)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0) return BadRequest(new { Message = "Company context not found." });

            if (payload == null || string.IsNullOrWhiteSpace(payload.BoardName))
                return BadRequest(new { Message = "Board name is required." });

            try
            {
                var board = await _kanbanService.CreateBoardAsync(cid, payload.BoardName.Trim());
                return Ok(board);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // ─── Columns & Tasks ─────────────────────────────────────────────────────────

        /// <summary>Returns columns (with nested tasks) for the specified board.</summary>
        [HttpGet]
        public async Task<IActionResult> GetColumns([FromQuery] int? companyId, [FromQuery] long boardId)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0) return BadRequest(new { Message = "Company context not found." });
            if (boardId <= 0) return BadRequest(new { Message = "boardId is required." });

            try
            {
                var columns = await _kanbanService.GetColumnsAsync(cid, boardId);
                return Ok(columns);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>Creates a column or a task depending on whether kanId is present.</summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromQuery] int? companyId, [FromQuery] long boardId, [FromBody] System.Text.Json.JsonElement payload)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0) return BadRequest(new { Message = "Company context not found." });
            if (boardId <= 0) return BadRequest(new { Message = "boardId is required." });

            try
            {
                if (payload.TryGetProperty("kanId", out var kanIdProp)
                    && kanIdProp.ValueKind != System.Text.Json.JsonValueKind.Null
                    && kanIdProp.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                {
                    var task = System.Text.Json.JsonSerializer.Deserialize<KanbanTaskModel>(payload.GetRawText());
                    if (task == null) return BadRequest(new { Message = "Invalid task data." });
                    await _kanbanService.SaveTaskAsync(cid, boardId, task);
                    return Ok(task);
                }
                else
                {
                    var col = System.Text.Json.JsonSerializer.Deserialize<KanbanColumnModel>(payload.GetRawText());
                    if (col == null) return BadRequest(new { Message = "Invalid column data." });
                    await _kanbanService.SaveColumnAsync(cid, boardId, col);
                    return Ok(col);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromQuery] int? companyId, [FromQuery] long boardId, [FromBody] KanbanTaskModel task)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0) return BadRequest(new { Message = "Company context not found." });
            if (boardId <= 0) return BadRequest(new { Message = "boardId is required." });
            if (task == null || string.IsNullOrEmpty(task.Id)) return BadRequest(new { Message = "Invalid task data." });

            try
            {
                await _kanbanService.SaveTaskAsync(cid, boardId, task);
                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int? companyId, [FromQuery] long boardId, [FromHeader] string card)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0) return BadRequest(new { Message = "Company context not found." });
            if (boardId <= 0) return BadRequest(new { Message = "boardId is required." });
            if (string.IsNullOrEmpty(card)) return BadRequest(new { Message = "Invalid card ID." });

            try
            {
                await _kanbanService.DeleteTaskAsync(cid, boardId, card);
                return Ok(card);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("column/{columnId}")]
        public async Task<IActionResult> DeleteColumn(string columnId, [FromQuery] int? companyId, [FromQuery] long boardId)
        {
            int cid = GetCompanyId(companyId);
            if (cid == 0) return BadRequest(new { Message = "Company context not found." });
            if (boardId <= 0) return BadRequest(new { Message = "boardId is required." });
            if (string.IsNullOrEmpty(columnId)) return BadRequest(new { Message = "Invalid column ID." });

            try
            {
                var result = await _kanbanService.DeleteColumnAsync(cid, boardId, columnId);
                return Ok(new { Success = result, ColumnId = columnId });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // ─── Checklists ──────────────────────────────────────────────────────────────

        [HttpGet("checklist")]
        public async Task<IActionResult> GetChecklistItems([FromQuery] long taskId)
        {
            try
            {
                var items = await _kanbanService.GetChecklistItemsAsync(taskId);
                return Ok(items);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
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
                return BadRequest(new { Message = "Invalid checklist item data." });

            try
            {
                if (item.ChecklistItemId <= 0)
                    item.CreatedBy = _tenantProvider.UserId;
                else
                    item.UpdatedBy = _tenantProvider.UserId;

                var savedItem = await _kanbanService.SaveChecklistItemAsync(item);
                return Ok(savedItem);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
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
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
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
                return BadRequest(new { Message = "Invalid parameters." });

            try
            {
                var result = await _kanbanService.DeleteChecklistGroupAsync(taskId, groupName);
                return Ok(new { Success = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
