using Domain.Linking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Participant
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }

        public List<FilmParticipation> Participations { get; set; } = new();
    }
}
