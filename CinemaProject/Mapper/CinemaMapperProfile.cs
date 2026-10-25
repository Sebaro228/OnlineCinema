using AutoMapper;
using Domain.DTOs;
using Domain.Models;

namespace CinemaProject.Mapper
{
    public class CinemaMapperProfile : Profile
    {
        public CinemaMapperProfile()
        {
            CreateMap<FilmDto, Film>();
            CreateMap<TrailerDto, Trailer>();
        }
    }
}
