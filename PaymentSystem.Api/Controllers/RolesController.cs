using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.AppRoleDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class RolesController : ControllerBase
    {
        readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
       
        [HttpGet("get-all")]
        public IActionResult GetAllRoles()
        {
            var result = _roleService.GetAll();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllRolesForAdmin()
        {
            var result = _roleService.GetAllForAdmin();
            return Ok(result);
        }
      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var result = await _roleService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetRoleForEdit(string id)
        {
            var result = await _roleService.GetForEditAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] AppRoleCreateDto dto)
        {
            var result = await _roleService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRole([FromBody] AppRoleUpdateDto dto)
        {
            var result = await _roleService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await _roleService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteRolesById(List<string> ids)
        {
            var result = await _roleService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(string id)
        {
            var result = await _roleService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(string id)
        {
            var result = await _roleService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var result = await _roleService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(string id)
        {
            var result = await _roleService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
