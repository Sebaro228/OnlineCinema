using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Linking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FilmParticipationService : Service<FilmParticipation>, IFilmParticipationService
    {
        public FilmParticipationService(IFilmParticipationRepository repository) : base(repository) { }
    }
}
