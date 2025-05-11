using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Interfaces;
using Football.Domain.Entities;
using Football.Domain.Interfaces;

namespace Football.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IMapper _mapper;

        public WeatherService(IWeatherRepository weatherRepository, IMapper mapper)
        {
            _weatherRepository = weatherRepository;
            _mapper = mapper;
        }

        public async Task<WeatherDto?> GetByGameAsync(int gameId)
        {
            var weather = await _weatherRepository.GetByGameIdAsync(gameId);
            return weather == null
                ? null
                : _mapper.Map<WeatherDto>(weather);
        }

        public async Task<WeatherDto> CreateAsync(int gameId, WeatherDto weatherDto)
        {
            if (weatherDto == null) throw new ArgumentNullException(nameof(weatherDto));
            var entity = _mapper.Map<Weather>(weatherDto);
            entity.GameId = gameId;
            await _weatherRepository.AddAsync(entity);

            var created = await _weatherRepository.GetByGameIdAsync(gameId);
            return _mapper.Map<WeatherDto>(created!);
        }

        public async Task UpdateAsync(int gameId, WeatherDto weatherDto)
        {
            if (weatherDto == null) throw new ArgumentNullException(nameof(weatherDto));
            var existing = await _weatherRepository.GetByGameIdAsync(gameId);
            if (existing == null)
                throw new KeyNotFoundException("Weather not found for game " + gameId);

            _mapper.Map(weatherDto, existing);
            await _weatherRepository.UpdateAsync(existing);
        }
    }
}