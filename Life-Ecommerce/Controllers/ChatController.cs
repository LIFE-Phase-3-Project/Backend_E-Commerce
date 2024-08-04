using Application.Services.Chat;
using Domain.DTOs.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {

        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        //[HttpPost("start-session")]
        //public async Task<IActionResult> StartSession([FromBody] StartSessionRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.CustomerEmail) || string.IsNullOrEmpty(request.AdminEmail))
        //        return BadRequest("Invalid request data.");

        //    var session = await _chatService.StartSessionAsync(request.CustomerEmail, request.AdminEmail);
        //    return Ok(new { sessionId = session.Id });
        //}

        [HttpPost("start-session")]
        public async Task<IActionResult> StartSession([FromBody] StartSessionRequest request)
        {
            if (string.IsNullOrEmpty(request.CustomerEmail) || string.IsNullOrEmpty(request.AdminEmail))
                return BadRequest("Invalid request data.");

            var session = await _chatService.StartSessionAsync(request.CustomerEmail, request.AdminEmail);
            return Ok(new { sessionId = session.Id });
        }

        [HttpGet("messages/{sessionId}")]
        public async Task<IActionResult> GetMessages(int sessionId)
        {
            var messages = await _chatService.GetMessagesAsync(sessionId);
            return Ok(messages);
        }

        [HttpPut("update-session-status/{sessionId}")]
        public async Task<IActionResult> UpdateSessionStatus(int sessionId)
        {
           

            try
            {
                await _chatService.UpdateSessionStatusAsync(sessionId);
                return Ok(new { message = "Session status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("pending-sessions")]
        public async Task<IActionResult> GetPendingSessionsWithMessages()
        {
            var sessions = await _chatService.GetPendingSessions();
            return Ok(sessions);
        }

        [HttpPost("end-session")]
        public async Task<IActionResult> EndSessionAsync([FromQuery] int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest("Invalid session ID.");
            }

            try
            {
                var result = await _chatService.EndSessionAsync(sessionId);
                if (result == null)
                {
                    return NotFound("Session does not exist.");
                }
                else if (!result)
                {
                    return BadRequest("Session has already ended.");
                }
                return Ok("Session ended successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
