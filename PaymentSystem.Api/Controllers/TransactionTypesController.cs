using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionTypeDto;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class TransactionTypesController : ControllerBase
    {
        readonly ITransactionTypeService _transactionTypeService;
        public TransactionTypesController(ITransactionTypeService transactionTypeService)
        {
            _transactionTypeService = transactionTypeService;
        }
      
        [HttpGet("get-all")]
        public IActionResult GetAllTransactionTypes()
        {
            var result = _transactionTypeService.GetAllIncluding();
            return Ok(result);
        }
       
        [HttpGet("get-by-transaction")]
        public IActionResult GetAllTransactionTypesByTransactions()
        {
            var result = _transactionTypeService.GetAllIncludingOrderByTransactions();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllTransactionTypesForAdmin()
        {
            var result = _transactionTypeService.GetAllIncludingForAdmin();
            return Ok(result);
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionTypeById(int id)
        {
            var result = await _transactionTypeService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
      
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetTransactionTypeForEdit(int id)
        {
            var result = await _transactionTypeService.GetByIdForUpdateAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransactionType([FromBody] TransactionTypeCreateDto dto)
        {
            var result = await _transactionTypeService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTransactionType([FromBody] TransactionTypeUpdateDto dto)
        {
            var result = await _transactionTypeService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionType(int id)
        {
            var result = await _transactionTypeService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteTransactionTypesById(List<int> ids)
        {
            var result = await _transactionTypeService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _transactionTypeService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _transactionTypeService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _transactionTypeService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _transactionTypeService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
