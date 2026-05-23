using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchLineupRepository _matchLineupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<MatchLineupService> _logger;

        public MatchLineupService(
            IMatchLineupRepository matchLineupRepository,
            IMatchRepository matchRepository,
            IPlayerRepository playerRepository,
            ILogger<MatchLineupService> logger)
        {
            _matchLineupRepository = matchLineupRepository;
            _matchRepository = matchRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<MatchLineup> AddToLineupAsync(int matchId, int playerId, bool isStarter, string position)
        {
            _logger.LogInformation("Adding player {PlayerId} to match {MatchId}", playerId, matchId);

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            var player = await _playerRepository.GetByIdWithTeamAsync(playerId);
            if (player == null)
                throw new KeyNotFoundException($"No se encontró el jugador con ID {playerId}");

            var belongsToMatch = (match.HomeTeamId == player.TeamId) || (match.AwayTeamId == player.TeamId);
            if (!belongsToMatch)
                throw new InvalidOperationException("El jugador no pertenece a ninguno de los equipos del partido");

            var exists = await _matchLineupRepository.ExistsByMatchAndPlayerAsync(matchId, playerId);
            if (exists)
                throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");

            if (isStarter)
            {
                var startersCount = await _matchLineupRepository.CountStartersByMatchAndTeamAsync(matchId, player.TeamId);
                if (startersCount >= 11)
                    throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
            }

            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled");

            var matchLineup = new MatchLineup
            {
                MatchId = matchId,
                PlayerId = playerId,
                IsStarter = isStarter,
                Position = position
            };

            var created = await _matchLineupRepository.CreateAsync(matchLineup);
            // ✅ Sin SaveChangesAsync — ya lo hace CreateAsync internamente

            _logger.LogInformation("Player {PlayerId} added to match {MatchId} successfully", playerId, matchId);
            return created;
        }

        public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId)
        {
            _logger.LogInformation("Retrieving lineup for match {MatchId}", matchId);
            return await _matchLineupRepository.GetByMatchIdAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAndTeamAsync(int matchId, int teamId)
        {
            _logger.LogInformation("Retrieving lineup for match {MatchId} and team {TeamId}", matchId, teamId);
            return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task<bool> RemoveFromLineupAsync(int id)
        {
            _logger.LogInformation("Removing lineup with ID {LineupId}", id);

            var lineup = await _matchLineupRepository.GetByIdAsync(id);
            if (lineup == null)
                return false;

            await _matchLineupRepository.DeleteAsync(id);
            // ✅ Sin SaveChangesAsync — ya lo hace DeleteAsync internamente

            _logger.LogInformation("Lineup with ID {LineupId} removed successfully", id);
            return true;
        }
    }
}
