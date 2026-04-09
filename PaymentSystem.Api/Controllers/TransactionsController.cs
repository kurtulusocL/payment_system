using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class TransactionsController : ControllerBase
    {
        readonly ITransactionService _transactionService;
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
      
        [HttpGet("get-all")]
        public IActionResult GetAllTransactions()
        {
            var result = _transactionService.GetAllIncluding();
            return Ok(result);
        }

        [HttpGet("get-by-wallet/{walletId}")]
        public IActionResult GetAllTransactionsByWalletId(int walletId)
        {
            var result = _transactionService.GetAllIncludingByWalletId(walletId);
            return Ok(result);
        }
       
        [HttpGet("get-by-payment/{paymentId?}")]
        public IActionResult GetAllTransactionsByPaymentId(int? paymentId)
        {
            var result = _transactionService.GetAllIncludingByPaymentId(paymentId);
            return Ok(result);
        }

        [HttpGet("get-by-currency/{currencyId}")]
        public IActionResult GetAllTransactionsByCurrencyId(int currencyId)
        {
            var result = _transactionService.GetAllIncludingByCurrencyId(currencyId);
            return Ok(result);
        }
       
        [HttpGet("get-by-type/{transactionTypeId}")]
        public IActionResult GetAllTransactionsByTransactionTypeId(int transactionTypeId)
        {
            var result = _transactionService.GetAllIncludingByTransactionTypeId(transactionTypeId);
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllTransactionsForAdmin()
        {
            var result = _transactionService.GetAllIncludingForAdmin();
            return Ok(result);
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var result = await _transactionService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetTransactionForEdit(int id)
        {
            var result = await _transactionService.GetByIdForUpdate(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDto dto)
        {
            var result = await _transactionService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTransaction([FromBody] TransactionUpdateDto dto)
        {
            var result = await _transactionService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var result = await _transactionService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteTransactionsById(List<int> ids)
        {
            var result = await _transactionService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _transactionService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _transactionService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _transactionService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _transactionService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
