using AutoMapper;
using Domain.DTOs;
using Domain.Linking;
using Domain.Models;

namespace CinemaProject.Mapper
{
    public class CinemaMapperProfile : Profile
    {
        public CinemaMapperProfile()
        {
            CreateMap<FilmDto, Film>();
            CreateMap<TrailerDto, Trailer>();
            CreateMap<ParticipantDto, Participant>();
            CreateMap<RegisterRequest, User>();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<FilmParticipationDto, FilmParticipation>();
        }
    }
}
