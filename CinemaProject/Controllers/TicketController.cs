using BLL.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController(TicketService ticketService) : ControllerBase
    {
        [HttpPost("CreateTicket")]
        public async Task<IActionResult> CreateTicket(CreateTicket request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            await ticketService.CreateTicket(request, userId);
            return Ok();
        }
        [HttpPost("AcceptTicket/{ticketId}")]
        public async Task<IActionResult> AcceptTicket(string ticketId, [FromBody] MessageDto message)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (agentId == null) return Unauthorized();
            var res = await ticketService.AcceptTicket(ticketId, agentId, message);
            if (!res.success) return BadRequest(res.message);
            return Ok();
        }
        [HttpPost("Responce/{ticketId}")]
        public async Task<IActionResult> Responce(string ticketId, [FromBody] MessageDto message)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (agentId == null) return Unauthorized();
            var res = await ticketService.Responce(ticketId, agentId, message);
            if (!res.success) return BadRequest(res.message);
            return Ok();
        }
        [HttpPost("CloseTicket/{ticketId}")]
        public async Task<IActionResult> CloseTicket(string ticketId)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (agentId == null) return Unauthorized();
            var res = await ticketService.CloseTicket(ticketId, agentId);
            if (!res.success) return BadRequest(res.message);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("HandleEmailReceived")]
        public async Task<IActionResult> HandleEmailReceived([FromBody] ResendWebhookPayload payload)
        {
            var res = await ticketService.HandleEmailReceived(payload);
            if (!res.success) return BadRequest(res.message);
            return Ok();
        }
    }
}
