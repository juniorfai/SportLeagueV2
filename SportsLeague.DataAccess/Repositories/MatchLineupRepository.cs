using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.DataAccess.Repositories
{
    public class MatchLineupRepository : GenericRepository<MatchLineup>, IMatchLineupRepository
    {
        public MatchLineupRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchIdAsync(int matchId)
        {
            return await _dbSet
                .Include(ml => ml.Player)
                    .ThenInclude(p => p!.Team)
                .Include(ml => ml.Match)
                .Where(ml => ml.MatchId == matchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _dbSet
                .Include(ml => ml.Player)
                    .ThenInclude(p => p!.Team)
                .Where(ml => ml.MatchId == matchId &&
                            ml.Player != null &&
                            ml.Player.TeamId == teamId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByMatchAndPlayerAsync(int matchId, int playerId)
        {
            return await _dbSet.AnyAsync(ml => ml.MatchId == matchId && ml.PlayerId == playerId);
        }

        public async Task<int> CountStartersByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _dbSet
                .CountAsync(ml => ml.MatchId == matchId &&
                                 ml.IsStarter == true &&
                                 ml.Player != null &&
                                 ml.Player.TeamId == teamId);
        }

        public async Task<MatchLineup?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(ml => ml.Player)
                    .ThenInclude(p => p!.Team)
                .FirstOrDefaultAsync(ml => ml.Id == id);
        }
    }
}
