using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Services;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Moq;

namespace Football.Tests.Services
{
    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly WeatherService _service;

        public WeatherServiceTests()
        {
            _repoMock = new Mock<IWeatherRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new WeatherService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByGameAsync_Existing_ShouldReturnWeatherDto()
        {
            // Arrange
            int gameId = 1;
            var weather = new Weather { Id = 1, GameId = gameId };
            var weatherDto = new WeatherDto { GameId = gameId };
            _repoMock.Setup(r => r.GetByGameIdAsync(gameId)).ReturnsAsync(weather);
            _mapperMock.Setup(m => m.Map<WeatherDto>(weather)).Returns(weatherDto);

            // Act
            var result = await _service.GetByGameAsync(gameId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(weatherDto.GameId, result.GameId);
        }

        [Fact]
        public async Task GetByGameAsync_Nonexistent_ShouldReturnNull()
        {
            // Arrange
            int gameId = 999;
            _repoMock.Setup(r => r.GetByGameIdAsync(gameId)).ReturnsAsync((Weather)null);

            // Act
            var result = await _service.GetByGameAsync(gameId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallRepositoryAndReturnDto()
        {
            // Arrange
            int gameId = 1;
            var inputDto = new WeatherDto { GameId = gameId, Temperature = 20, RainChance = 0.1, WindSpeed = 5, Icon = "sunny" };
            var newEntity = new Weather { Id = 100, GameId = gameId, Temperature = 20, RainChance = 0.1, WindSpeed = 5, Icon = "sunny" };
            var savedEntity = new Weather { Id = 100, GameId = gameId, Temperature = 20, RainChance = 0.1, WindSpeed = 5, Icon = "sunny" };
            _mapperMock.Setup(m => m.Map<Weather>(inputDto)).Returns(newEntity);
            _repoMock.Setup(r => r.AddAsync(newEntity)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetByGameIdAsync(gameId)).ReturnsAsync(savedEntity);
            var outputDto = new WeatherDto { GameId = gameId, Temperature = 20, RainChance = 0.1, WindSpeed = 5, Icon = "sunny" };
            _mapperMock.Setup(m => m.Map<WeatherDto>(savedEntity)).Returns(outputDto);

            // Act
            var result = await _service.CreateAsync(gameId, inputDto);

            // Assert
            Assert.Equal(outputDto.GameId, result.GameId);
            Assert.Equal(outputDto.Temperature, result.Temperature);
            Assert.Equal(outputDto.RainChance, result.RainChance);
            Assert.Equal(outputDto.WindSpeed, result.WindSpeed);
            Assert.Equal(outputDto.Icon, result.Icon);
        }

        [Fact]
        public async Task CreateAsync_NullDto_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(() => _service.CreateAsync(1, null));
        }

        [Fact]
        public async Task UpdateAsync_Nonexistent_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            int gameId = 1;
            var dto = new WeatherDto { GameId = gameId, Temperature = 10, RainChance = 0.2, WindSpeed = 3, Icon = "cloudy" };
            _repoMock.Setup(r => r.GetByGameIdAsync(gameId)).ReturnsAsync((Weather)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(gameId, dto));
        }

        [Fact]
        public async Task UpdateAsync_Existing_ShouldUpdateWeather()
        {
            // Arrange
            int gameId = 1;
            var dto = new WeatherDto { GameId = gameId, Temperature = 15, RainChance = 0.3, WindSpeed = 7, Icon = "rainy" };
            var weather = new Weather { Id = 1, GameId = gameId, Temperature = 10, RainChance = 0.2, WindSpeed = 3, Icon = "cloudy" };
            _repoMock.Setup(r => r.GetByGameIdAsync(gameId)).ReturnsAsync(weather);
            _mapperMock.Setup(m => m.Map(dto, weather)).Returns(weather);
            _repoMock.Setup(r => r.UpdateAsync(weather)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(gameId, dto);

            // Assert
            _repoMock.Verify(r => r.GetByGameIdAsync(gameId), Times.Once);
            _mapperMock.Verify(m => m.Map(dto, weather), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(weather), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NullDto_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(() => _service.UpdateAsync(1, null));
        }
    }
}
