using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantStatusDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class MerchantStatusesController : ControllerBase
    {
        readonly IMerchantStatusService _merchantStatusService;
        public MerchantStatusesController(IMerchantStatusService merchantStatusService)
        {
            _merchantStatusService = merchantStatusService;
        }
       
        [HttpGet("get-all")]
        public IActionResult GetAllMerchantStatuses()
        {
            var result = _merchantStatusService.GetAllIncluding();
            return Ok(result);
        }
       
        [HttpGet("get-by-merchant")]
        public IActionResult GetAllMerchantStatusesByMerchants()
        {
            var result = _merchantStatusService.GetAllIncludingOrderByMerchants();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllMerchantStatusesForAdmin()
        {
            var result = _merchantStatusService.GetAllIncludingForAdmin();
            return Ok(result);
        }
      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMerchantStatusById(int id)
        {
            var result = await _merchantStatusService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetMerchantStatusForEdit(int id)
        {
            var result = await _merchantStatusService.GetByIdForUpdateAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreateMerchantStatus([FromBody] MerchantStatusCreateDto dto)
        {
            var result = await _merchantStatusService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateMerchantStatus([FromBody] MerchantStatusUpdateDto dto)
        {
            var result = await _merchantStatusService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMerchantStatus(int id)
        {
            var result = await _merchantStatusService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMerchantStatusesById(List<int> ids)
        {
            var result = await _merchantStatusService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _merchantStatusService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _merchantStatusService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _merchantStatusService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _merchantStatusService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
