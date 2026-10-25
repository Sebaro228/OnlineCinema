using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class TrailerDto
    {
        public int FilmId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
    }
}
