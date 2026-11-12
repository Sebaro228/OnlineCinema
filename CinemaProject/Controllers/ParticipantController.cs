using AutoMapper;
using BLL.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _service;
        private readonly IMapper _mapper;

        public ParticipantController(IParticipantService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateParticipant(ParticipantDto dto) 
        {
            if (string.IsNullOrEmpty(dto.FullName))
                return BadRequest("A field \"FullName\" can not be empty");
            if (dto.Age == 0)
                return BadRequest("A field \"Age\" can not equal 0");

            Participant p = _mapper.Map<Participant>(dto);
            return Ok(p);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteParticipant(string id)
        {
            Participant? p = await _service.GetAsync(id);
            if (p is null)
                return NotFound();

            await _service.DeleteAsync(p);
            return Ok();
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetParticipant(string id)
        {
            Participant? p = await _service.GetAsync(id);
            if (p is null)
                return NotFound();
            return Ok(p);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllParticipants()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateParticipant(string id, ParticipantDto dto)
        {
            Participant? p = await _service.GetAsync(id);
            if (p is null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.FullName))
                p.FullName = dto.FullName;
            if (dto.Age != 0)
                p.Age = dto.Age;

            return Ok();
        }
    }
}
