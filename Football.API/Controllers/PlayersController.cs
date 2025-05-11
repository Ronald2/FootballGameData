using Football.Application.DTOs;
using Football.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace Football.API.Controllers
{
    [ApiController]
    [Route("api/teams/{teamId}/players")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        /// <summary>
        /// Obtiene una lista paginada de jugadores de un equipo.
        /// </summary>
        /// <param name="teamId">ID del equipo.</param>
        /// <param name="page">Número de página (opcional).</param>
        /// <param name="pageSize">Tamaño de página (opcional).</param>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<PlayerDto>>> ListByTeam(
            int teamId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _playerService.ListByTeamAsync(teamId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Crea un nuevo jugador en un equipo.
        /// </summary>
        /// <param name="teamId">ID del equipo.</param>
        /// <param name="dto">DTO del jugador a crear.</param>
        [HttpPost]
        public async Task<ActionResult<PlayerDto>> Create(int teamId, [FromBody] PlayerDto dto)
        {
            var created = await _playerService.CreateAsync(teamId, dto);
            return CreatedAtAction(nameof(GetById), new { teamId, id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza un jugador existente.
        /// </summary>
        /// <param name="teamId">ID del equipo.</param>
        /// <param name="id">ID del jugador.</param>
        /// <param name="dto">DTO del jugador a actualizar.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int teamId, int id, [FromBody] PlayerDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _playerService.UpdateAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// Elimina un jugador por su identificador.
        /// </summary>
        /// <param name="teamId">ID del equipo.</param>
        /// <param name="id">ID del jugador.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int teamId, int id)
        {
            await _playerService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Obtiene un jugador por su identificador.
        /// </summary>
        /// <param name="teamId">ID del equipo.</param>
        /// <param name="id">ID del jugador.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetById(int teamId, int id)
        {
            var player = await _playerService.GetByIdAsync(id);
            if (player == null) return NotFound();
            return Ok(player);
        }

        /// <summary>
        /// Exporta los jugadores de un equipo en formato JSON.
        /// </summary>
        [HttpGet("export/json")]
        public async Task<IActionResult> ExportJson(int teamId, [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _playerService.ListByTeamAsync(teamId, page, pageSize);
            return File(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(result.Items), "application/json", $"players_team{teamId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
        }

        /// <summary>
        /// Exporta los jugadores de un equipo en formato CSV.
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv(int teamId, [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _playerService.ListByTeamAsync(teamId, page, pageSize);
            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(result.Items);
            }
            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "text/csv", $"players_team{teamId}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }
    }
}