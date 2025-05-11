using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Services;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace Football.Tests.Services
{
    public class LineUpServiceTests
    {
        private readonly Mock<ILineUpRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOptions<PaginationOptions>> _paginationOptionsMock;
        private readonly LineUpService _service;

        public LineUpServiceTests()
        {
            _repoMock = new Mock<ILineUpRepository>();
            _mapperMock = new Mock<IMapper>();
            _paginationOptionsMock = new Mock<IOptions<PaginationOptions>>();
            _paginationOptionsMock.Setup(x => x.Value).Returns(new PaginationOptions());
            _service = new LineUpService(_repoMock.Object, _mapperMock.Object, _paginationOptionsMock.Object);
        }

        [Fact]
        public async Task ListByGameAsync_ShouldReturnPagedResult_WithCorrectData()
        {
            // Arrange
            int gameId = 1, page = 1, pageSize = 2;
            var lineUps = new List<LineUp> { new LineUp { Id = 1 }, new LineUp { Id = 2 } };
            int totalCount = 5;
            _repoMock.Setup(r => r.GetPagedByGameAsync(gameId, page, pageSize)).ReturnsAsync((lineUps, totalCount));
            var lineUpDtos = new List<LineUpDto> { new LineUpDto { Id = 1 }, new LineUpDto { Id = 2 } };
            _mapperMock.Setup(m => m.Map<IEnumerable<LineUpDto>>(lineUps)).Returns(lineUpDtos);

            // Act
            var result = await _service.ListByGameAsync(gameId, page, pageSize);

            // Assert
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(lineUpDtos, result.Items);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, -1)]
        public async Task ListByGameAsync_InvalidArguments_ShouldNormalizeOrThrow(int page, int pageSize)
        {
            // Arrange
            int gameId = 1;
            var lineUps = new List<LineUp>();
            int totalCount = 0;
            _repoMock.Setup(r => r.GetPagedByGameAsync(gameId, 1, 20)).ReturnsAsync((lineUps, totalCount));
            _mapperMock.Setup(m => m.Map<IEnumerable<LineUpDto>>(lineUps)).Returns(new List<LineUpDto>());

            // Act
            var result = await _service.ListByGameAsync(gameId, page, pageSize);

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.PageSize);
            Assert.Empty(result.Items);
            Assert.Equal(totalCount, result.TotalCount);
        }

        [Fact]
        public async Task GetByIdAsync_Existing_ShouldReturnLineUpDto()
        {
            // Arrange
            int id = 1;
            var lineUp = new LineUp { Id = id };
            var lineUpDto = new LineUpDto { Id = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(lineUp);
            _mapperMock.Setup(m => m.Map<LineUpDto>(lineUp)).Returns(lineUpDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(lineUpDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_Nonexistent_ShouldReturnNull()
        {
            // Arrange
            int id = 999;
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((LineUp)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallRepositoryAndReturnDto()
        {
            // Arrange
            int gameId = 1;
            var inputDto = new LineUpDto { Id = 0 };
            var newEntity = new LineUp { Id = 100, GameId = gameId, PlayerId = 1, TeamId = 1 };
            var savedEntity = new LineUp { Id = 100, GameId = gameId, PlayerId = 1, TeamId = 1 };
            _mapperMock.Setup(m => m.Map<LineUp>(inputDto)).Returns(newEntity);
            _repoMock.Setup(r => r.AddAsync(newEntity)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetByIdAsync(savedEntity.Id)).ReturnsAsync(savedEntity);
            var outputDto = new LineUpDto { Id = savedEntity.Id };
            _mapperMock.Setup(m => m.Map<LineUpDto>(savedEntity)).Returns(outputDto);

            // Act
            var result = await _service.CreateAsync(gameId, inputDto);

            // Assert
            Assert.Equal(outputDto, result);
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
            var dto = new LineUpDto { Id = 200 };
            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((LineUp)null);

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
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((LineUp)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
        }

        [Fact]
        public async Task UpdateAsync_Existing_ShouldUpdateLineUp()
        {
            // Arrange
            var dto = new LineUpDto { Id = 1 };
            var lineUp = new LineUp { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(lineUp);
            _mapperMock.Setup(m => m.Map(dto, lineUp)).Returns(lineUp);
            _repoMock.Setup(r => r.UpdateAsync(lineUp)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            _repoMock.Verify(r => r.GetByIdAsync(dto.Id), Times.Once);
            _mapperMock.Verify(m => m.Map(dto, lineUp), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(lineUp), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Existing_ShouldDeleteLineUp()
        {
            // Arrange
            int id = 1;
            var lineUp = new LineUp { Id = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(lineUp);
            _repoMock.Setup(r => r.DeleteAsync(lineUp)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.DeleteAsync(lineUp), Times.Once);
        }
    }
}
