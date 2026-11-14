using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Linking
{
    public class FilmParticipation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ParticipantId { get; set; } = string.Empty;
        [JsonIgnore]
        public Participant Participant { get; set; } = new();

        public string FilmId { get; set; } = string.Empty;
        [JsonIgnore]
        public Film Film { get; set; } = new();

        public string ParticipantRole { get; set; } = string.Empty;
    }
}
