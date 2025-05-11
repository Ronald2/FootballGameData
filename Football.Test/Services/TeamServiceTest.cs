using AutoMapper;
using Football.Application.DTOs;
using Football.Application.Services;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace Football.Test.Services
{
    public class TeamServiceTests
    {
        private readonly Mock<ITeamRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOptions<PaginationOptions>> _paginationOptionsMock;
        private readonly TeamService _service;

        public TeamServiceTests()
        {
            _repoMock = new Mock<ITeamRepository>();
            _mapperMock = new Mock<IMapper>();
            _paginationOptionsMock = new Mock<IOptions<PaginationOptions>>();
            _paginationOptionsMock.Setup(x => x.Value).Returns(new PaginationOptions());
            _service = new TeamService(_repoMock.Object, _mapperMock.Object, _paginationOptionsMock.Object);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnPagedResult_WithCorrectData()
        {
            // Arrange
            int page = 2, pageSize = 3;
            var teams = new List<Team>
            {
                new Team { Id = 1, Tricode = "T1", Name = "Team One", Coach = "Coach A" },
                new Team { Id = 2, Tricode = "T2", Name = "Team Two", Coach = "Coach B" }
            };
            int totalCount = 10;

            _repoMock
                .Setup(r => r.GetPagedAsync(page, pageSize, null, null))
                .ReturnsAsync((teams, totalCount));

            var teamDtos = new List<TeamDto>
            {
                new TeamDto { Id = 1, Tricode = "T1", Name = "Team One", Coach = "Coach A" },
                new TeamDto { Id = 2, Tricode = "T2", Name = "Team Two", Coach = "Coach B" }
            };

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TeamDto>>(teams))
                .Returns(teamDtos);

            // Act
            var result = await _service.ListAsync(page, pageSize);

            // Assert
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(teamDtos, result.Items);

            _repoMock.Verify(r => r.GetPagedAsync(page, pageSize, null, null), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<TeamDto>>(teams), Times.Once);
        }

        [Fact]
        public async Task ListAsync_InvalidPageAndSize_ShouldNormalizeValues()
        {
            // Arrange
            int invalidPage = 0, invalidSize = -5;
            var teams = new List<Team>();
            int totalCount = 0;

            // Expect defaults page=1, pageSize=20
            _repoMock
                .Setup(r => r.GetPagedAsync(1, 20, null, null))
                .ReturnsAsync((teams, totalCount));

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TeamDto>>(teams))
                .Returns(new List<TeamDto>());

            // Act
            var result = await _service.ListAsync(invalidPage, invalidSize);

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.PageSize);
            Assert.Empty(result.Items);
            Assert.Equal(totalCount, result.TotalCount);

            _repoMock.Verify(r => r.GetPagedAsync(1, 20, null, null), Times.Once);
        }

        [Fact]
        public async Task ListAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            int page = 1, pageSize = 5;
            _repoMock
                .Setup(r => r.GetPagedAsync(page, pageSize, null, null))
                .ThrowsAsync(new System.Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _service.ListAsync(page, pageSize));
        }

        [Fact]
        public async Task ListAsync_MapperThrowsException_ShouldPropagateException()
        {
            // Arrange
            int page = 1, pageSize = 5;
            var teams = new List<Team> { new Team { Id = 1, Name = "A", Tricode = "A", Coach = "C" } };
            int totalCount = 1;
            _repoMock
                .Setup(r => r.GetPagedAsync(page, pageSize, null, null))
                .ReturnsAsync((teams, totalCount));
            _mapperMock
                .Setup(m => m.Map<IEnumerable<TeamDto>>(teams))
                .Throws(new System.Exception("Mapper error"));

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _service.ListAsync(page, pageSize));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTeamDto_WhenTeamExists()
        {
            // Arrange
            int teamId = 1;
            var team = new Team { Id = teamId, Name = "Team One", Tricode = "T1", Coach = "Coach A" };
            var teamDto = new TeamDto { Id = teamId, Name = "Team One", Tricode = "T1", Coach = "Coach A" };
            _repoMock.Setup(r => r.GetByIdAsync(teamId)).ReturnsAsync(team);
            _mapperMock.Setup(m => m.Map<TeamDto>(team)).Returns(teamDto);

            // Act
            var result = await _service.GetByIdAsync(teamId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(teamDto, result);
            _repoMock.Verify(r => r.GetByIdAsync(teamId), Times.Once);
            _mapperMock.Verify(m => m.Map<TeamDto>(team), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTeamDoesNotExist()
        {
            // Arrange
            int teamId = 99;
            _repoMock.Setup(r => r.GetByIdAsync(teamId)).ReturnsAsync((Team)null);

            // Act
            var result = await _service.GetByIdAsync(teamId);

            // Assert
            Assert.Null(result);
            _repoMock.Verify(r => r.GetByIdAsync(teamId), Times.Once);
        }

        [Fact]
        public async Task ListAsync_EmptyRepository_ShouldReturnEmptyResult()
        {
            // Arrange
            int page = 1, pageSize = 10, totalCount = 0;
            var teams = new List<Team>();
            var teamDtos = new List<TeamDto>();
            _repoMock.Setup(r => r.GetPagedAsync(page, pageSize, null, null)).ReturnsAsync((teams, totalCount));
            _mapperMock.Setup(m => m.Map<IEnumerable<TeamDto>>(teams)).Returns(teamDtos);

            // Act
            var result = await _service.ListAsync(page, pageSize);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
        }

        [Fact]
        public async Task ListAsync_WithFilters_ShouldPassFiltersToRepository()
        {
            // Arrange
            int page = 1, pageSize = 10, totalCount = 1;
            string nameFilter = "Team", coachFilter = "Coach";
            var teams = new List<Team> { new Team { Id = 1, Name = "Team", Tricode = "T1", Coach = "Coach" } };
            var teamDtos = new List<TeamDto> { new TeamDto { Id = 1, Name = "Team", Tricode = "T1", Coach = "Coach" } };
            _repoMock.Setup(r => r.GetPagedAsync(page, pageSize, nameFilter, coachFilter)).ReturnsAsync((teams, totalCount));
            _mapperMock.Setup(m => m.Map<IEnumerable<TeamDto>>(teams)).Returns(teamDtos);

            // Act
            var result = await _service.ListAsync(page, pageSize, nameFilter, coachFilter);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
            _repoMock.Verify(r => r.GetPagedAsync(page, pageSize, nameFilter, coachFilter), Times.Once);
        }
    }
}