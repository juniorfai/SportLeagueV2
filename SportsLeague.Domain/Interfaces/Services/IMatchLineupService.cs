
using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchLineupService
    {
        Task<MatchLineup> AddToLineupAsync(int matchId, int playerId, bool isStarter, string position);
        Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId);
        Task<IEnumerable<MatchLineup>> GetLineupByMatchAndTeamAsync(int matchId, int teamId);
        Task<bool> RemoveFromLineupAsync(int id);
    }
}
