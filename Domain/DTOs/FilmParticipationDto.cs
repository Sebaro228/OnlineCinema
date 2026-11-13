using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class FilmParticipationDto
    {
        public string ParticipantId { get; set; } = string.Empty;
        public string FilmId { get; set; } = string.Empty;
        public string ParticipantRole { get; set; } = string.Empty;
    }
}
