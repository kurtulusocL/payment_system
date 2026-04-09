using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Application.Constants.Messages;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Infrastructure.Constants.Attributes;

namespace PaymentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AllAdmins")]
    [AuditLog]
    [ExceptionHandler]
    public class UsersController : ControllerBase
    {
        readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
       
        [HttpGet("get-all")]
        public IActionResult GetAllUsers()
        {
            var result = _userService.GetAllIncluding();
            return Ok(result);
        }
      
        [HttpGet("get-inactive")]
        public IActionResult GetAllInactiveUsers()
        {
            var result = _userService.GetAllIncludingDeActiveUser();
            return Ok(result);
        }

        [HttpGet("get-deleted")]
        public IActionResult GetAllDeletedUsers()
        {
            var result = _userService.GetAllIncludingDeletedUser();
            return Ok(result);
        }
       
        [HttpGet("get-active")]
        public IActionResult GetAllActiveUsers()
        {
            var result = _userService.GetAllIncludingActiveUser();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllUsersForAdmin()
        {
            var result = _userService.GetAllIncludingForAdmin();
            return Ok(result);
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteUsersById(List<string> ids)
        {
            var result = await _userService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(string id)
        {
            var result = await _userService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(string id)
        {
            var result = await _userService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var result = await _userService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(string id)
        {
            var result = await _userService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
