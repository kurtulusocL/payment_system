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
    public class UserSessionsController : ControllerBase
    {
        readonly IUserSessionService _userSessionService;
        public UserSessionsController(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
        }
      
        [HttpGet("get-all")]
        public IActionResult GetAllUserSessions()
        {
            var result = _userSessionService.GetAllIncluding();
            return Ok(result);
        }
      
        [HttpGet("get-online")]
        public IActionResult GetAllOnlineUserSessions()
        {
            var result = _userSessionService.GetAllIncludingByOnline();
            return Ok(result);
        }
       
        [HttpGet("get-by-user/{userId}")]
        public IActionResult GetAllUserSessionsByUserId(string userId)
        {
            var result = _userSessionService.GetAllIncludingByUserId(userId);
            return Ok(result);
        }
        
        [HttpGet("get-by-login-date")]
        public IActionResult GetAllUserSessionsByLoginDate()
        {
            var result = _userSessionService.GetAllIncludingByLoginDate();
            return Ok(result);
        }
       
        [HttpGet("get-by-duration")]
        public IActionResult GetAllUserSessionsByOnlineDuration()
        {
            var result = _userSessionService.GetAllIncludingByOnlineDurationTime();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllUserSessionsForAdmin()
        {
            var result = _userSessionService.GetAllIncludingForAdmin();
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserSessionById(int id)
        {
            var result = await _userSessionService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSession(int id)
        {
            var result = await _userSessionService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteUserSessionsById(List<int> ids)
        {
            var result = await _userSessionService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _userSessionService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _userSessionService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _userSessionService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _userSessionService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
