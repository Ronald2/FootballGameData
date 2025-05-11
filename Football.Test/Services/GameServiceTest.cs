using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Services;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace Football.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOptions<PaginationOptions>> _paginationOptionsMock;
        private readonly GameService _service;

        public GameServiceTests()
        {
            _repoMock = new Mock<IGameRepository>();
            _mapperMock = new Mock<IMapper>();
            _paginationOptionsMock = new Mock<IOptions<PaginationOptions>>();
            _paginationOptionsMock.Setup(x => x.Value).Returns(new PaginationOptions());
            _service = new GameService(_repoMock.Object, _mapperMock.Object, _paginationOptionsMock.Object);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnPagedResult_WithCorrectData()
        {
            // Arrange
            int page = 1, pageSize = 2;
            var games = new List<Game> { new Game { Id = 1 }, new Game { Id = 2 } };
            int totalCount = 5;
            _repoMock.Setup(r => r.GetPagedAsync(page, pageSize)).ReturnsAsync((games, totalCount));
            var gameDtos = new List<GameDto> { new GameDto { Id = 1 }, new GameDto { Id = 2 } };
            _mapperMock.Setup(m => m.Map<IEnumerable<GameDto>>(games)).Returns(gameDtos);

            // Act
            var result = await _service.ListAsync(page, pageSize);

            // Assert
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(gameDtos, result.Items);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, -1)]
        public async Task ListAsync_InvalidArguments_ShouldNormalizeOrThrow(int page, int pageSize)
        {
            // Arrange
            var games = new List<Game>();
            int totalCount = 0;
            _repoMock.Setup(r => r.GetPagedAsync(1, 20)).ReturnsAsync((games, totalCount));
            _mapperMock.Setup(m => m.Map<IEnumerable<GameDto>>(games)).Returns(new List<GameDto>());

            // Act
            var result = await _service.ListAsync(page, pageSize);

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.PageSize);
            Assert.Empty(result.Items);
            Assert.Equal(totalCount, result.TotalCount);
        }

        [Fact]
        public async Task GetByIdAsync_Existing_ShouldReturnGameDto()
        {
            // Arrange
            int id = 1;
            var game = new Game { Id = id };
            var gameDto = new GameDto { Id = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(game);
            _mapperMock.Setup(m => m.Map<GameDto>(game)).Returns(gameDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_Nonexistent_ShouldReturnNull()
        {
            // Arrange
            int id = 999;
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Game)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallRepositoryAndReturnDto()
        {
            // Arrange
            var inputDto = new GameDto { Id = 0 };
            var newEntity = new Game { Id = 100 };
            var savedEntity = new Game { Id = 100 };
            _mapperMock.Setup(m => m.Map<Game>(inputDto)).Returns(newEntity);
            _repoMock.Setup(r => r.AddAsync(newEntity)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetByIdAsync(savedEntity.Id)).ReturnsAsync(savedEntity);
            var outputDto = new GameDto { Id = savedEntity.Id };
            _mapperMock.Setup(m => m.Map<GameDto>(savedEntity)).Returns(outputDto);

            // Act
            var result = await _service.CreateAsync(inputDto);

            // Assert
            Assert.Equal(outputDto, result);
        }

        [Fact]
        public async Task CreateAsync_NullDto_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(() => _service.CreateAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_Nonexistent_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var dto = new GameDto { Id = 200 };
            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Game)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_NullDto_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(() => _service.UpdateAsync(null));
        }

        [Fact]
        public async Task DeleteAsync_Nonexistent_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            int id = 300;
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Game)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
        }

        [Fact]
        public async Task UpdateAsync_Existing_ShouldUpdateGame()
        {
            // Arrange
            var dto = new GameDto { Id = 1 };
            var game = new Game { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(game);
            _mapperMock.Setup(m => m.Map(dto, game)).Returns(game);
            _repoMock.Setup(r => r.UpdateAsync(game)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            _repoMock.Verify(r => r.GetByIdAsync(dto.Id), Times.Once);
            _mapperMock.Verify(m => m.Map(dto, game), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(game), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Existing_ShouldDeleteGame()
        {
            // Arrange
            int id = 1;
            var game = new Game { Id = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(game);
            _repoMock.Setup(r => r.DeleteAsync(game)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.DeleteAsync(game), Times.Once);
        }
    }
}
