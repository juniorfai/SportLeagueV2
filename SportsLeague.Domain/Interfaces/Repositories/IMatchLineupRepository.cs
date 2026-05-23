using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface IMatchLineupRepository : IGenericRepository<MatchLineup>
    {
        Task<IEnumerable<MatchLineup>> GetByMatchIdAsync(int matchId);
        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);
        Task<bool> ExistsByMatchAndPlayerAsync(int matchId, int playerId);
        Task<int> CountStartersByMatchAndTeamAsync(int matchId, int teamId);
        Task<MatchLineup?> GetByIdWithDetailsAsync(int id);
    }
}
