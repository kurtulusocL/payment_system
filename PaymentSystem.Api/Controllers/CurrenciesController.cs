using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class CurrenciesController : ControllerBase
    {
        readonly ICurrencyService _currencyService;
        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        
        [HttpGet("get-all")]
        public IActionResult GetAllCurrencies()
        {
            var result = _currencyService.GetAllIncluding();
            return Ok(result);
        }
       
        [HttpGet("get-by-payment")]
        public IActionResult GetAllCurrenciesByPayment()
        {
            var result = _currencyService.GetAllIncludingOrderByPayment();
            return Ok(result);
        }
      
        [HttpGet("get-by-wallet")]
        public IActionResult GetAllCurrenciesByWallet()
        {
            var result = _currencyService.GetAllIncludingOrderByWallet();
            return Ok(result);
        }
        
        [HttpGet("get-by-transaction")]
        public IActionResult GetAllCurrenciesByTransaction()
        {
            var result = _currencyService.GetAllIncludingOrderByTransaction();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllCurrenciesForAdmin()
        {
            var result = _currencyService.GetAllIncludingForAdmin();
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrencyById(int id)
        {
            var result = await _currencyService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetCurrencyForEdit(int id)
        {
            var result = await _currencyService.GetByIdForUpdate(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreateCurrency([FromBody] CurrencyCreateDto dto)
        {
            var result = await _currencyService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCurrency([FromBody] CurrencyUpdateDto dto)
        {
            var result = await _currencyService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var result = await _currencyService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteCurrenciesById(List<int> ids)
        {
            var result = await _currencyService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _currencyService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _currencyService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _currencyService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _currencyService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
