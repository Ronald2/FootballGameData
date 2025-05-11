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
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        /// <summary>
        /// Obtiene una lista paginada de equipos, con filtros opcionales por nombre y coach.
        /// </summary>
        /// <param name="page">Número de página (opcional).</param>
        /// <param name="pageSize">Tamaño de página (opcional).</param>
        /// <param name="name">Filtrar por nombre de equipo.</param>
        /// <param name="coach">Filtrar por nombre del coach.</param>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TeamDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? name = null,
            [FromQuery] string? coach = null)
        {
            var result = await _teamService.ListAsync(page, pageSize, name, coach);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un equipo por su identificador.
        /// </summary>
        /// <param name="id">ID del equipo.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetById(int id)
        {
            var team = await _teamService.GetByIdAsync(id);
            if (team == null) return NotFound();
            return Ok(team);
        }

        /// <summary>
        /// Crea un nuevo equipo.
        /// </summary>
        /// <param name="teamDto">DTO del equipo a crear.</param>
        [HttpPost]
        public async Task<ActionResult<TeamDto>> Create([FromBody] TeamDto teamDto)
        {
            var created = await _teamService.CreateAsync(teamDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza un equipo existente.
        /// </summary>
        /// <param name="id">ID del equipo.</param>
        /// <param name="teamDto">DTO del equipo a actualizar.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TeamDto teamDto)
        {
            if (id != teamDto.Id) return BadRequest();
            await _teamService.UpdateAsync(teamDto);
            return NoContent();
        }

        /// <summary>
        /// Elimina un equipo por su identificador.
        /// </summary>
        /// <param name="id">ID del equipo.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _teamService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Exporta todos los equipos en formato JSON.
        /// </summary>
        [HttpGet("export/json")]
        public async Task<IActionResult> ExportJson([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _teamService.ListAsync(page, pageSize);
            return File(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(result.Items), "application/json", $"teams_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
        }

        /// <summary>
        /// Exporta todos los equipos en formato CSV.
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var result = await _teamService.ListAsync(page, pageSize);
            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(result.Items);
            }
            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "text/csv", $"teams_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }
    }
}