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
    public class OutboxEventsController : ControllerBase
    {
        readonly IOutboxEventService _outboxEventService;
        public OutboxEventsController(IOutboxEventService outboxEventService)
        {
            _outboxEventService = outboxEventService;
        }
       
        [HttpGet("get-all")]
        public IActionResult GetAllOutboxEvents()
        {
            var result = _outboxEventService.GetAll();
            return Ok(result);
        }
       
        [HttpGet("get-successful")]
        public IActionResult GetAllSuccessfulOutboxEvents()
        {
            var result = _outboxEventService.GetAllBySuccessfullProcess();
            return Ok(result);
        }
       
        [HttpGet("get-failed")]
        public IActionResult GetAllFailedOutboxEvents()
        {
            var result = _outboxEventService.GetAllByErrorProcess();
            return Ok(result);
        }
       
        [HttpGet("get-all-admin")]
        public IActionResult GetAllOutboxEventsForAdmin()
        {
            var result = _outboxEventService.GetAllForAdmin();
            return Ok(result);
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutboxEventById(int id)
        {
            var result = await _outboxEventService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOutboxEvent(int id)
        {
            var result = await _outboxEventService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteOutboxEventsById(List<int> ids)
        {
            var result = await _outboxEventService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.DeleteError);
            return Ok(MessageConstants.DeleteSuccess);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _outboxEventService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsActiveError);
            return Ok(MessageConstants.IsActiveSuccess);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInactive(int id)
        {
            var result = await _outboxEventService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsInActiveError);
            return Ok(MessageConstants.IsInActiveSuccess);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _outboxEventService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.IsDeletedError);
            return Ok(MessageConstants.IsDeletedSuccess);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _outboxEventService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(MessageConstants.NotDeleteError);
            return Ok(MessageConstants.NotDeletedSuccess);
        }
    }
}
