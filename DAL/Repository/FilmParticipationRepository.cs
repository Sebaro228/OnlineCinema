using DAL.Context;
using DAL.Interfaces;
using Domain.Linking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class FilmParticipationRepository : Repository<FilmParticipation>, IFilmParticipationRepository
    {
        public FilmParticipationRepository(CinemaContext context) : base(context) { }
    }
}
