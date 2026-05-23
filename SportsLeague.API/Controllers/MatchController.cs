using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match/{matchId}/lineup")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _matchLineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(
            IMatchLineupService matchLineupService,
            IMapper mapper)
        {
            _matchLineupService = matchLineupService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MatchLineupResponseDTO>> AddToLineup(
            int matchId,
            [FromBody] CreateMatchLineupRequestDTO request)
        {
            try
            {
                var lineup = await _matchLineupService.AddToLineupAsync(
                    matchId,
                    request.PlayerId,
                    request.IsStarter,
                    request.Position);

                var response = _mapper.Map<MatchLineupResponseDTO>(lineup);
                return CreatedAtAction(
                    nameof(GetLineupByMatch),
                    new { matchId },
                    response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineupByMatch(int matchId)
        {
            var lineups = await _matchLineupService.GetLineupByMatchAsync(matchId);
            var response = _mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups);
            return Ok(response);
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineupByTeam(
            int matchId,
            int teamId)
        {
            var lineups = await _matchLineupService.GetLineupByMatchAndTeamAsync(matchId, teamId);
            var response = _mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromLineup(int matchId, int id)
        {
            var result = await _matchLineupService.RemoveFromLineupAsync(id);
            if (!result)
                return NotFound(new { message = $"No se encontró la alineación con ID {id}" });

            return NoContent();
        }
    }
}