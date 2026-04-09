using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class PaymentsController : ControllerBase
    {
        readonly IPaymentService _paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllPayments()
        {
            var result = _paymentService.GetAllIncluding();
            return Ok(result);
        }
      
        [HttpGet("get-by-user/{userId}")]
        public IActionResult GetAllPaymentsByUserId(string userId)
        {
            var result = _paymentService.GetAllIncludingByUserId(userId);
            return Ok(result);
        }
       
        [HttpGet("get-by-status/{statusId}")]
        public IActionResult GetAllPaymentsByStatusId(int statusId)
        {
            var result = _paymentService.GetAllIncludingByPaymentStatusId(statusId);
            return Ok(result);
        }
       
        [HttpGet("get-by-currency/{currencyId}")]
        public IActionResult GetAllPaymentsByCurrencyId(int currencyId)
        {
            var result = _paymentService.GetAllIncludingByCurrencyId(currencyId);
            return Ok(result);
        }
       
        [HttpGet("get-by-merchant/{merchantId}")]
        public IActionResult GetAllPaymentsByMerchantId(int merchantId)
        {
            var result = _paymentService.GetAllIncludingByMerchantId(merchantId);
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllPaymentsForAdmin()
        {
            var result = _paymentService.GetAllIncludingForAdmin();
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var result = await _paymentService.GetByIdAsync(id);
            if (result == null || !result.Any())
                return NotFound();
            return Ok(result);
        }
       
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetPaymentForEdit(int id)
        {
            var result = await _paymentService.GetByIdForUpdateAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            var result = await _paymentService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePayment([FromBody] PaymentUpdateDto dto)
        {
            var result = await _paymentService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeletePaymentsById(List<int> ids)
        {
            var result = await _paymentService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _paymentService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _paymentService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _paymentService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _paymentService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
