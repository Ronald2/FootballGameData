using Football.Application.DTOs;

namespace Football.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerDto?> GetByIdAsync(int id);
        Task<PlayerDto> CreateAsync(int teamId, PlayerDto playerDto);
        Task UpdateAsync(PlayerDto playerDto);
        Task DeleteAsync(int id);
        Task<PagedResultDto<PlayerDto>> ListByTeamAsync(int teamId, int page = 1, int pageSize = 20);

    }
}