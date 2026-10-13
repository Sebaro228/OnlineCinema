using DAL.Context;
using DAL.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class FilmRepository : Repository<Film>
    {
        public FilmRepository(CinemaContext context) : base(context) { }
    }
}
