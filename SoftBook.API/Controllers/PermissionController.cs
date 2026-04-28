using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/permission")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _repository;

        public PermissionController(IPermissionRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<List<RolePermission>>> GetByRole(int roleId)
        {
            try
            {
                var permissions = await _repository.GetPermissionsByRoleAsync(roleId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("modules")]
        public async Task<ActionResult<List<SidebarItem>>> GetModules()
        {
            try
            {
                var modules = await _repository.GetAllModulesAsync();
                return Ok(modules);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePermissions([FromBody] RolePermissionUpdateDTO updateDto)
        {
            try
            {
                if (updateDto == null || updateDto.RoleId <= 0)
                    return BadRequest(new { error = "Invalid role ID or permission data" });

                var success = await _repository.SaveRolePermissionsAsync(updateDto);
                if (success)
                    return Ok(new { message = "Permissions updated successfully" });
                
                return StatusCode(500, new { error = "An error occurred while saving permissions" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
