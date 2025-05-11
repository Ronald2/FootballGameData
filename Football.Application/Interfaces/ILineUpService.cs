using Football.Application.DTOs;

namespace Football.Application.Interfaces
{
    public interface ILineUpService
    { 
        Task<PagedResultDto<LineUpDto>> ListByGameAsync(int gameId, int page = 1, int pageSize = 20);

        Task<LineUpDto?> GetByIdAsync(int id);

        Task<LineUpDto> CreateAsync(int gameId, LineUpDto lineUpDto);

        Task UpdateAsync(LineUpDto lineUpDto);

        Task DeleteAsync(int id);
        

    }
}