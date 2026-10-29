using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Trailer
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Mime { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int FilmId { get; set; }
        [JsonIgnore]
        public Film Film { get; set; }
    }
}
