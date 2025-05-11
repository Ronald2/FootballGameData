using Football.Application.DTOs;
using Football.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace Football.API.Controllers
{
    [ApiController]
    [Route("api/games/{gameId}/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Obtiene el clima para un partido.
        /// </summary>
        /// <param name="gameId">ID del partido.</param>
        [HttpGet]
        public async Task<ActionResult<WeatherDto>> Get(int gameId)
        {
            var weather = await _weatherService.GetByGameAsync(gameId);
            if (weather == null) return NotFound();
            return Ok(weather);
        }

        /// <summary>
        /// Crea el clima para un partido.
        /// </summary>
        /// <param name="gameId">ID del partido.</param>
        /// <param name="weatherDto">DTO del clima a crear.</param>
        [HttpPost]
        public async Task<ActionResult<WeatherDto>> Create(int gameId, [FromBody] WeatherDto weatherDto)
        {
            var created = await _weatherService.CreateAsync(gameId, weatherDto);
            return CreatedAtAction(nameof(Get), new { gameId }, created);
        }

        /// <summary>
        /// Actualiza el clima de un partido.
        /// </summary>
        /// <param name="gameId">ID del partido.</param>
        /// <param name="weatherDto">DTO del clima a actualizar.</param>
        [HttpPut]
        public async Task<IActionResult> Update(int gameId, [FromBody] WeatherDto weatherDto)
        {
            await _weatherService.UpdateAsync(gameId, weatherDto);
            return NoContent();
        }

        /// <summary>
        /// Exporta el clima de un partido en formato JSON.
        /// </summary>
        [HttpGet("export/json")]
        public async Task<IActionResult> ExportJson(int gameId)
        {
            var weather = await _weatherService.GetByGameAsync(gameId);
            if (weather == null) return NotFound();
            var list = new List<WeatherDto> { weather };
            return File(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(list), "application/json", $"weather_game{gameId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
        }

        /// <summary>
        /// Exporta el clima de un partido en formato CSV.
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv(int gameId)
        {
            var weather = await _weatherService.GetByGameAsync(gameId);
            if (weather == null) return NotFound();
            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(new List<WeatherDto> { weather });
            }
            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "text/csv", $"weather_game{gameId}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }
    }
}