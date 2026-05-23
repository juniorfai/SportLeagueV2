using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Entities
{
    public class MatchLineup : AuditBase
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public bool IsStarter { get; set; }
        public string Position { get; set; } = string.Empty;

        // Relaciones de navegación
        public virtual Match? Match { get; set; }
        public virtual Player? Player { get; set; }
    }
}
