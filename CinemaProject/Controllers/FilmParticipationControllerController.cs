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
            if (!await _participantService.AnyAsync(p => string.Equals(p.Id, dto.ParticipantId)))
                return BadRequest("There is no participant with this id");

            if (!await _filmService.AnyAsync(f => string.Equals(f.Id, dto.FilmId)))
                return BadRequest("There is no film with this id");

            if (string.IsNullOrEmpty(dto.ParticipantRole))
                return BadRequest("The role can not be empty");

            FilmParticipation p = _mapper.Map<FilmParticipation>(dto);
            await _participationService.CreateAsync(p);
            return Ok(p);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteFilmParticipation(string id)
        {
            FilmParticipation? p = await _participationService.GetAsync(id);
            if (p is null)
                return NotFound();

            await _participationService.DeleteAsync(p);
            return Ok();
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetFilmParticipation(string id)
        {
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
            FilmParticipation? par = await _participationService.GetAsync(id);
            if (par is null)
                return NotFound();
            string prevFilmId = par.FilmId;

            if (!string.IsNullOrEmpty(dto.FilmId))
            {
                if(!await _filmService.AnyAsync(f => string.Equals(f.Id, dto.FilmId)))
                    return BadRequest("There is no film with this id");

                par.FilmId = dto.FilmId;
            }

            if (!string.IsNullOrEmpty(dto.ParticipantId))
            {
                if (!await _participantService.AnyAsync(p => string.Equals(p.Id, dto.ParticipantId)))
                {
                    par.FilmId = prevFilmId;
                    return BadRequest("There is no participant with this id");
                }

                par.ParticipantId = dto.ParticipantId;
            }

            if(!string.IsNullOrEmpty(dto.ParticipantRole))
                par.ParticipantRole = dto.ParticipantRole;

            await _participationService.UpdateAsync(par);
            return Ok();
        }
    }
}
