using DAL.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FilmService : Service<Film>
    {
        public FilmService(IRepository<Film> rep) : base(rep) { }
    }
}
