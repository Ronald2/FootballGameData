using Football.Application.DTOs;
using Football.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace Football.API.Controllers
{
    [ApiController]
    [Route("api/games/{gameId}/lineups")]
    public class LineUpsController : ControllerBase
    {
        private readonly ILineUpService _lineUpService;

        public LineUpsController(ILineUpService lineUpService)
        {
            _lineUpService = lineUpService;
        }

        /// <summary>
        /// Obtiene una lista paginada de alineaciones para un partido.
        /// </summary>
        /// <param name="gameId">ID del partido.</param>
        /// <param name="page">Número de página (opcional).</param>
        /// <param name="pageSize">Tamaño de página (opcional).</param>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<LineUpDto>>> ListByGame(
            int gameId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _lineUpService.ListByGameAsync(gameId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una alineación por su identificador.
        /// </summary>
        /// <param name="id">ID de la alineación.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<LineUpDto>> GetById(int id)
        {
            var lineUp = await _lineUpService.GetByIdAsync(id);
            if (lineUp == null) return NotFound();
            return Ok(lineUp);
        }

        /// <summary>
        /// Crea una nueva alineación para un partido.
        /// </summary>
        /// <param name="gameId">ID del partido.</param>
        /// <param name="lineUpDto">DTO de la alineación a crear.</param>
        [HttpPost]
        public async Task<ActionResult<LineUpDto>> Create(int gameId, [FromBody] LineUpDto lineUpDto)
        {
            var created = await _lineUpService.CreateAsync(gameId, lineUpDto);
            return CreatedAtAction(nameof(GetById), new { gameId = gameId, id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza una alineación existente.
        /// </summary>
        /// <param name="id">ID de la alineación.</param>
        /// <param name="lineUpDto">DTO de la alineación a actualizar.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LineUpDto lineUpDto)
        {
            if (id != lineUpDto.Id) return BadRequest();
            await _lineUpService.UpdateAsync(lineUpDto);
            return NoContent();
        }

        /// <summary>
        /// Elimina una alineación por su identificador.
        /// </summary>
        /// <param name="id">ID de la alineación.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _lineUpService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Exporta las alineaciones de un partido en formato JSON.
        /// </summary>
        [HttpGet("export/json")]
        public async Task<IActionResult> ExportJson(int gameId, [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _lineUpService.ListByGameAsync(gameId, page, pageSize);
            return File(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(result.Items), "application/json", $"lineups_game{gameId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
        }

        /// <summary>
        /// Exporta las alineaciones de un partido en formato CSV.
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv(int gameId, [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _lineUpService.ListByGameAsync(gameId, page, pageSize);
            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(result.Items);
            }
            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "text/csv", $"lineups_game{gameId}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }
    }
}