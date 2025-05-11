using Football.Application.DTOs;

namespace Football.Application.Interfaces
{
    public interface IGameService
    {
        Task<IEnumerable<GameDto>> ListAllAsync();
        Task<GameDto?> GetByIdAsync(int id);
        Task<GameDto> CreateAsync(GameDto gameDto);
        Task UpdateAsync(GameDto gameDto);
        Task DeleteAsync(int id);
        Task<PagedResultDto<GameDto>> ListAsync(int page = 1, int pageSize = 20);
    }
}