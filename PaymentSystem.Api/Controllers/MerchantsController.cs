using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class MerchantsController : ControllerBase
    {
        readonly IMerchantService _merchantService;
        public MerchantsController(IMerchantService merchantService)
        {
            _merchantService = merchantService;
        }
        
        [HttpGet("get-all")]
        public IActionResult GetAllMerchants()
        {
            var result = _merchantService.GetAllIncluding();
            return Ok(result);
        }
       
        [HttpGet("get-by-status/{statusId}")]
        public IActionResult GetAllMerchantsByStatusId(int statusId)
        {
            var result = _merchantService.GetAllIncludingByStatusId(statusId);
            return Ok(result);
        }
        
        [HttpGet("get-null-task-numbers")]
        public IActionResult GetAllMerchantsByNullTaskNumber()
        {
            var result = _merchantService.GetAllIncludingByNullTaskNumber();
            return Ok(result);
        }
       
        [HttpGet("get-by-payment")]
        public IActionResult GetAllMerchantsByPayment()
        {
            var result = _merchantService.GetAllIncludingByPayment();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllMerchantsForAdmin()
        {
            var result = _merchantService.GetAllIncludingForAdmin();
            return Ok(result);
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMerchantById(int id)
        {
            var result = await _merchantService.GetByIdAsync(id);
            if (result == null || !result.Any())
                return NotFound();
            return Ok(result);
        }
        
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetMerchantForEdit(int id)
        {
            var result = await _merchantService.GetByIdForUpdateAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateMerchant([FromBody] MerchantCreateDto dto)
        {
            var result = await _merchantService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateMerchant([FromBody] MerchantUpdateDto dto)
        {
            var result = await _merchantService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMerchant(int id)
        {
            var result = await _merchantService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMerchantsById(List<int> ids)
        {
            var result = await _merchantService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _merchantService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _merchantService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _merchantService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _merchantService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
