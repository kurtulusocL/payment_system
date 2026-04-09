using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;
using PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class WalletsController : ControllerBase
    {
        readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }
       
        [HttpGet("get-all")]
        public IActionResult GetAllWallets()
        {
            var result = _walletService.GetAllIncluding();
            return Ok(result);
        }
               
        [HttpGet("get-by-user/{userId}")]
        public IActionResult GetAllWalletsByUserId(string userId)
        {
            var result = _walletService.GetAllIncludingByUserId(userId);
            return Ok(result);
        }
      
        [HttpGet("get-by-currency/{currencyId}")]
        public IActionResult GetAllWalletsByCurrencyId(int currencyId)
        {
            var result = _walletService.GetAllIncludingByCurrencyId(currencyId);
            return Ok(result);
        }
        
        [HttpGet("get-all-admin")]
        public IActionResult GetAllWalletsForAdmin()
        {
            var result = _walletService.GetAllIncludingForAdmin();
            return Ok(result);
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalletById(int id)
        {
            var result = await _walletService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpGet("get-for-edit/{id}")]
        public async Task<IActionResult> GetWalletForEdit(int id)
        {
            var result = await _walletService.GetByIdForUpdateAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet([FromBody] WalletCreateDto dto)
        {
            var result = await _walletService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.AddError);
            return Ok(MessageConstants.AddSuccess);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateWallet([FromBody] WalletUpdateDto dto)
        {
            var result = await _walletService.UpdateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.UpdateError);
            return Ok(MessageConstants.UpdateSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var result = await _walletService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteWalletsById(List<int> ids)
        {
            var result = await _walletService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _walletService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _walletService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _walletService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _walletService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
