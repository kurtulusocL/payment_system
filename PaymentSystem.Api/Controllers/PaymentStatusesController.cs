using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentStatusDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class PaymentStatusesController : ControllerBase
    {
        readonly IPaymentStatusService _paymentStatusService;
        public PaymentStatusesController(IPaymentStatusService paymentStatusService)
        {
            _paymentStatusService = paymentStatusService;
        }
       
        [HttpGet("get-all")]
        public IActionResult GetAllPaymentStatuses()
        {
            var result = _paymentStatusService.GetAllIncluding();
            return Ok(result);
        }

        [HttpGet("get-by-payment")]
        public IActionResult GetAllPaymentStatusesByPayments()
        {
            var result = _paymentStatusService.GetAllIncludingOrderByPayments();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllPaymentStatusesForAdmin()
        {
            var result = _paymentStatusService.GetAllIncludingForAdmin();
            return Ok(result);
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentStatusById(int id)
        {
            var result = await _paymentStatusService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetPaymentStatusForEdit(int id)
        {
            var result = await _paymentStatusService.GetByIdForUpdateAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreatePaymentStatus([FromBody] PaymentStatusCreateDto dto)
        {
            var result = await _paymentStatusService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] PaymentStatusUpdateDto dto)
        {
            var result = await _paymentStatusService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentStatus(int id)
        {
            var result = await _paymentStatusService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeletePaymentStatusesById(List<int> ids)
        {
            var result = await _paymentStatusService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _paymentStatusService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _paymentStatusService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _paymentStatusService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _paymentStatusService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
