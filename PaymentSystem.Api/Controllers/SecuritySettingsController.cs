using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class SecuritySettingsController : ControllerBase
    {
        readonly ISecuritySettingService _securitySettingService;
        public SecuritySettingsController(ISecuritySettingService securitySettingService)
        {
            _securitySettingService = securitySettingService;
        }
       
        [HttpGet("get-all")]
        public IActionResult GetAllSecuritySettings()
        {
            var result = _securitySettingService.GetAll();
            return Ok(result);
        }
      
        [HttpGet("get-all-admin")]
        public IActionResult GetAllSecuritySettingsForAdmin()
        {
            var result = _securitySettingService.GetAllForAdmin();
            return Ok(result);
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSecuritySettingById(int id)
        {
            var result = await _securitySettingService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetSecuritySettingForEdit(int id)
        {
            var result = await _securitySettingService.GetByIdForUpdateAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateSecuritySetting([FromBody] SecuritySettingCreateDto dto)
        {
            var result = await _securitySettingService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSecuritySetting([FromBody] SecuritySettingUpdateDto dto)
        {
            var result = await _securitySettingService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSecuritySetting(int id)
        {
            var result = await _securitySettingService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteSecuritySettingsById(List<int> ids)
        {
            var result = await _securitySettingService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _securitySettingService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _securitySettingService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _securitySettingService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _securitySettingService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
