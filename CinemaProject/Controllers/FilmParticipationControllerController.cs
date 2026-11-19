using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Domain.DTOs;
using Domain.Linking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmParticipationControllerController : ControllerBase
    {
        private readonly IFilmParticipationService _participationService;
        private readonly IFilmService _filmService;
        private readonly IParticipantService _participantService;
        private readonly IMapper _mapper;

        public FilmParticipationControllerController(
            IFilmParticipationService participationService,
            IFilmService filmService,
            IParticipantService participantService,
            IMapper mapper
        )
        {
            _participationService = participationService;
            _filmService = filmService;
            _participantService = participantService;
            _mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateFilmParticipation(FilmParticipationDto dto)
        {
            if (dto is null)
                return BadRequest("The participation data can not be null");
            if (string.IsNullOrWhiteSpace(dto.ParticipantId))
                return BadRequest("A participant id can not be empty");
            if (string.IsNullOrWhiteSpace(dto.FilmId))
                return BadRequest("A film id can not be empty");
            if (dto.ParticipantRoles is null || !dto.ParticipantRoles.Any())
                return BadRequest("Participant should have at least one role");
            if (dto.ParticipantRoles.Any(role => string.IsNullOrWhiteSpace(role)))
                return BadRequest("A participant role can not be empty");

            if (!await _participantService.AnyAsync(p => string.Equals(p.Id, dto.ParticipantId)))
                return BadRequest("There is no participant with this id");

            if (!await _filmService.AnyAsync(f => string.Equals(f.Id, dto.FilmId)))
                return BadRequest("There is no film with this id");

            FilmParticipation p = _mapper.Map<FilmParticipation>(dto);
            await _participationService.CreateAsync(p);
            return Ok(p);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteFilmParticipation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("A participation id can not be empty");

            FilmParticipation? p = await _participationService.GetAsync(id);
            if (p is null)
                return NotFound();

            await _participationService.DeleteAsync(p);
            return Ok();
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetFilmParticipation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("A participation id can not be empty");

            FilmParticipation? p = await _participationService.GetAsync(id);
            if (p is null)
                return NotFound();
            return Ok(p);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllFilmParticipations()
        {
            return Ok(await _participationService.GetAllAsync());
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateFilmParticipations(string id, FilmParticipationDto dto)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("A participation id can not be empty");
            if (dto is null)
                return BadRequest("The participation data can not be null");
            if (string.IsNullOrWhiteSpace(dto.FilmId) &&
                string.IsNullOrWhiteSpace(dto.ParticipantId) &&
                (dto.ParticipantRoles is null || !dto.ParticipantRoles.Any()))
                return BadRequest("At least one field must be provided for update");
            if (!string.IsNullOrEmpty(dto.FilmId) && string.IsNullOrWhiteSpace(dto.FilmId))
                return BadRequest("A film id can not be empty");
            if (!string.IsNullOrEmpty(dto.ParticipantId) && string.IsNullOrWhiteSpace(dto.ParticipantId))
                return BadRequest("A participant id can not be empty");
            if (dto.ParticipantRoles is not null &&
                dto.ParticipantRoles.Any(role => string.IsNullOrWhiteSpace(role)))
                return BadRequest("A participant role can not be empty");

            FilmParticipation? par = await _participationService.GetAsync(id);
            if (par is null)
                return NotFound();
            string prevFilmId = par.FilmId;

            if (!string.IsNullOrWhiteSpace(dto.FilmId))
            {
                if(!await _filmService.AnyAsync(f => string.Equals(f.Id, dto.FilmId)))
                    return BadRequest("There is no film with this id");

                par.FilmId = dto.FilmId;
            }

            if (!string.IsNullOrWhiteSpace(dto.ParticipantId))
            {
                if (!await _participantService.AnyAsync(p => string.Equals(p.Id, dto.ParticipantId)))
                {
                    par.FilmId = prevFilmId;
                    return BadRequest("There is no participant with this id");
                }

                par.ParticipantId = dto.ParticipantId;
            }

            if (dto.ParticipantRoles is not null && dto.ParticipantRoles.Any())
                par.ParticipantRoles = dto.ParticipantRoles;

            await _participationService.UpdateAsync(par);
            return Ok();
        }
    }
}
