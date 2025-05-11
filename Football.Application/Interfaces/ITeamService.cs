using Football.Application.DTOs;

namespace Football.Application.Interfaces
{
    public interface ITeamService
    {
        Task<PagedResultDto<TeamDto>> ListAsync(int page = 1, int pageSize = 20, string? nameFilter = null, string? coachFilter = null);
        Task<TeamDto?> GetByIdAsync(int id);
        Task<TeamDto> CreateAsync(TeamDto team);
        Task UpdateAsync(TeamDto team);
        Task DeleteAsync(int id);
    }
}