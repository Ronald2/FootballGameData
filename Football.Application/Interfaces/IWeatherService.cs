using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Football.Application.DTOs;

namespace Football.Application.Interfaces
{
    public interface IWeatherService
    {
         Task<WeatherDto?> GetByGameAsync(int gameId);

        Task<WeatherDto> CreateAsync(int gameId, WeatherDto weatherDto);

        Task UpdateAsync(int gameId, WeatherDto weatherDto);
    }
}