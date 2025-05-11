using Football.Application.DTOs;
using Football.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace Football.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Obtiene una lista paginada de partidos.
        /// </summary>
        /// <param name="page">Número de página (opcional).</param>
        /// <param name="pageSize">Tamaño de página (opcional).</param>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<GameDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _gameService.ListAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un partido por su identificador.
        /// </summary>
        /// <param name="id">ID del partido.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetById(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        /// <summary>
        /// Crea un nuevo partido.
        /// </summary>
        /// <param name="gameDto">DTO del partido a crear.</param>
        [HttpPost]
        public async Task<ActionResult<GameDto>> Create([FromBody] GameDto gameDto)
        {
            var created = await _gameService.CreateAsync(gameDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza un partido existente.
        /// </summary>
        /// <param name="id">ID del partido.</param>
        /// <param name="gameDto">DTO del partido a actualizar.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GameDto gameDto)
        {
            if (id != gameDto.Id) return BadRequest();
            await _gameService.UpdateAsync(gameDto);
            return NoContent();
        }

        /// <summary>
        /// Elimina un partido por su identificador.
        /// </summary>
        /// <param name="id">ID del partido.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _gameService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Exporta todos los partidos en formato JSON.
        /// </summary>
        [HttpGet("export/json")]
        public async Task<IActionResult> ExportJson([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _gameService.ListAsync(page, pageSize);
            return File(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(result.Items), "application/json", $"games_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
        }

        /// <summary>
        /// Exporta todos los partidos en formato CSV.
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _gameService.ListAsync(page, pageSize);
            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(result.Items);
            }
            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "text/csv", $"games_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }
    }
}