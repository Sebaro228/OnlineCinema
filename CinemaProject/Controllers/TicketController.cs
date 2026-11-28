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
            var ticket = await ticketService.CreateTicket(request, userId);
            if (ticket == null) return BadRequest("Open ticket already exists");
            return Ok(ticket);
        }
        [HttpPost("AcceptTicket/{ticketId}")]
        [Authorize(Roles = "SupportAgent")]
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
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return Unauthorized();
            var res = await ticketService.Responce(ticketId, id, message);
            if (!res.success) return BadRequest(res.message);
            return Ok();
        }
        [HttpPost("CloseTicket/{ticketId}")]
        public async Task<IActionResult> CloseTicket(string ticketId)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return Unauthorized();
            var res = await ticketService.CloseTicket(ticketId, id);
            if (!res.success) return BadRequest(res.message);
            return Ok();
        }
        [HttpGet("GetOpenedTickets")]
        [Authorize(Roles = "SupportAgent")]
        public async Task<IActionResult> GetOpenedTickets()
        {
            var tickets = await ticketService.GetOpenedTickets();
            return Ok(tickets);
        }
        [HttpGet("GetTicketByUserId")]
        public async Task<IActionResult> GetTicketByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var ticket = await ticketService.GetTicketByUserId(userId);
            if (ticket == null) return NotFound();
            return Ok(ticket);
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
