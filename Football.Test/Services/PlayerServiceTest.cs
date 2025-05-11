using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Services;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace Football.Tests.Services
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOptions<PaginationOptions>> _paginationOptionsMock;
        private readonly PlayerService _service;

        public PlayerServiceTests()
        {
            _repoMock = new Mock<IPlayerRepository>();
            _mapperMock = new Mock<IMapper>();
            _paginationOptionsMock = new Mock<IOptions<PaginationOptions>>();
            _paginationOptionsMock.Setup(x => x.Value).Returns(new PaginationOptions());
            _service = new PlayerService(_repoMock.Object, _mapperMock.Object, _paginationOptionsMock.Object);
        }

        [Fact]
        public async Task ListByTeamAsync_ShouldReturnPagedResult_WithCorrectData()
        {
            // Arrange
            int teamId = 42;
            int page = 2, pageSize = 3;
            var players = new List<Player>
            {
                new Player { Id = 1, TeamId = teamId, FirstName = "John", LastName = "Doe", Number = 7 },
                new Player { Id = 2, TeamId = teamId, FirstName = "Jane", LastName = "Smith", Number = 10 }
            };
            int totalCount = 5;

            _repoMock
                .Setup(r => r.GetPagedByTeamAsync(teamId, page, pageSize))
                .ReturnsAsync((players, totalCount));

            var playerDtos = new List<PlayerDto>
            {
                new PlayerDto { Id = 1, Number = 7, FirstName = "John", LastName = "Doe" },
                new PlayerDto { Id = 2, Number = 10, FirstName = "Jane", LastName = "Smith" }
            };

            _mapperMock
                .Setup(m => m.Map<IEnumerable<PlayerDto>>(players))
                .Returns(playerDtos);

            // Act
            var result = await _service.ListByTeamAsync(teamId, page, pageSize);

            // Assert
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(playerDtos, result.Items);

            _repoMock.Verify(r => r.GetPagedByTeamAsync(teamId, page, pageSize), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<PlayerDto>>(players), Times.Once);
        }

        [Fact]
        public async Task ListByTeamAsync_InvalidPageAndSize_ShouldNormalizeValues()
        {
            // Arrange
            int teamId = 42;
            int invalidPage = 0, invalidSize = -5;
            var players = new List<Player>();
            int totalCount = 0;

            // Expect defaults page=1, pageSize=20
            _repoMock
                .Setup(r => r.GetPagedByTeamAsync(teamId, 1, 20))
                .ReturnsAsync((players, totalCount));

            _mapperMock
                .Setup(m => m.Map<IEnumerable<PlayerDto>>(players))
                .Returns(new List<PlayerDto>());

            // Act
            var result = await _service.ListByTeamAsync(teamId, invalidPage, invalidSize);

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.PageSize);
            Assert.Empty(result.Items);
            Assert.Equal(totalCount, result.TotalCount);

            _repoMock.Verify(r => r.GetPagedByTeamAsync(teamId, 1, 20), Times.Once);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, -1)]
        public async Task ListByTeamAsync_InvalidArguments_ShouldNormalizeOrThrow(int page, int pageSize)
        {
            // Arrange
            int teamId = 42;
            var players = new List<Player>();
            int totalCount = 0;
            _repoMock.Setup(r => r.GetPagedByTeamAsync(teamId, 1, 20)).ReturnsAsync((players, totalCount));
            _mapperMock.Setup(m => m.Map<IEnumerable<PlayerDto>>(players)).Returns(new List<PlayerDto>());

            // Act
            var result = await _service.ListByTeamAsync(teamId, page, pageSize);

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.PageSize);
            Assert.Empty(result.Items);
            Assert.Equal(totalCount, result.TotalCount);
        }

        [Fact]
        public async Task ListByTeamAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            int teamId = 42, page = 1, pageSize = 10;
            _repoMock
                .Setup(r => r.GetPagedByTeamAsync(teamId, page, pageSize))
                .ThrowsAsync(new System.Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _service.ListByTeamAsync(teamId, page, pageSize));
        }

        [Fact]
        public async Task ListByTeamAsync_MapperThrowsException_ShouldPropagateException()
        {
            // Arrange
            int teamId = 42, page = 1, pageSize = 10;
            var players = new List<Player> { new Player { Id = 1, TeamId = teamId, FirstName = "A", LastName = "B", Number = 1 } };
            int totalCount = 1;
            _repoMock
                .Setup(r => r.GetPagedByTeamAsync(teamId, page, pageSize))
                .ReturnsAsync((players, totalCount));
            _mapperMock
                .Setup(m => m.Map<IEnumerable<PlayerDto>>(players))
                .Throws(new System.Exception("Mapper error"));

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _service.ListByTeamAsync(teamId, page, pageSize));
        }

        [Fact]
        public async Task CreateAsync_ShouldCallRepositoryAndReturnDto()
        {
            // Arrange
            int teamId = 42;
            var inputDto = new PlayerDto { Number = 9, FirstName = "Alex", LastName = "Morgan" };
            var newEntity = new Player { Id = 100, TeamId = teamId, Number = 9, FirstName = "Alex", LastName = "Morgan" };
            var savedEntity = new Player { Id = 100, TeamId = teamId, Number = 9, FirstName = "Alex", LastName = "Morgan" };

            _mapperMock
                .Setup(m => m.Map<Player>(inputDto))
                .Returns(newEntity);

            _repoMock
                .Setup(r => r.AddAsync(newEntity))
                .Returns(Task.CompletedTask)
                .Callback(() => newEntity.Id = savedEntity.Id);

            _repoMock
                .Setup(r => r.GetByIdAsync(savedEntity.Id))
                .ReturnsAsync(savedEntity);

            var outputDto = new PlayerDto { Id = savedEntity.Id, Number = 9, FirstName = "Alex", LastName = "Morgan" };
            _mapperMock
                .Setup(m => m.Map<PlayerDto>(savedEntity))
                .Returns(outputDto);

            // Act
            var result = await _service.CreateAsync(teamId, inputDto);

            // Assert
            Assert.Equal(outputDto, result);
            _mapperMock.Verify(m => m.Map<Player>(inputDto), Times.Once);
            _repoMock.Verify(r => r.AddAsync(newEntity), Times.Once);
            _repoMock.Verify(r => r.GetByIdAsync(savedEntity.Id), Times.Once);
            _mapperMock.Verify(m => m.Map<PlayerDto>(savedEntity), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_NullDto_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(() => _service.CreateAsync(42, null));
        }

        [Fact]
        public async Task GetByIdAsync_Nonexistent_ShouldReturnNull()
        {
            // Arrange
            int id = 999;
            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Player)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Existing_ShouldReturnPlayerDto()
        {
            // Arrange
            int id = 1;
            var player = new Player { Id = id, FirstName = "Test", LastName = "User", Number = 10, TeamId = 42 };
            var playerDto = new PlayerDto { Id = id, FirstName = "Test", LastName = "User", Number = 10 };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(player);
            _mapperMock.Setup(m => m.Map<PlayerDto>(player)).Returns(playerDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(playerDto, result);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mapperMock.Verify(m => m.Map<PlayerDto>(player), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Nonexistent_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var dto = new PlayerDto { Id = 200, Number = 11, FirstName = "Test", LastName = "User" };
            _repoMock
                .Setup(r => r.GetByIdAsync(dto.Id))
                .ReturnsAsync((Player)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_Existing_ShouldUpdatePlayer()
        {
            // Arrange
            var dto = new PlayerDto { Id = 1, Number = 11, FirstName = "Test", LastName = "User" };
            var player = new Player { Id = 1, Number = 11, FirstName = "Test", LastName = "User", TeamId = 42 };
            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(player);
            _mapperMock.Setup(m => m.Map(dto, player)).Returns(player);
            _repoMock.Setup(r => r.UpdateAsync(player)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            _repoMock.Verify(r => r.GetByIdAsync(dto.Id), Times.Once);
            _mapperMock.Verify(m => m.Map(dto, player), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(player), Times.Once);
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
            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Player)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
        }

        [Fact]
        public async Task DeleteAsync_Existing_ShouldDeletePlayer()
        {
            // Arrange
            int id = 1;
            var player = new Player { Id = id, FirstName = "Test", LastName = "User", Number = 10, TeamId = 42 };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(player);
            _repoMock.Setup(r => r.DeleteAsync(player)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.DeleteAsync(player), Times.Once);
        }
    }
}
