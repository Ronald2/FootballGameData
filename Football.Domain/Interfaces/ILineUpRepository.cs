namespace Football.Domain.Interfaces
{
    using Football.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILineUpRepository
    {
        Task<LineUp?> GetByIdAsync(int id);
        Task<IReadOnlyList<LineUp>> ListByGameAsync(int gameId);
        Task AddAsync(LineUp lineUp);
        Task UpdateAsync(LineUp lineUp);
        Task DeleteAsync(LineUp lineUp);
        Task<(IReadOnlyList<LineUp> Items, int TotalCount)> GetPagedByGameAsync(int gameId, int page, int pageSize);
    }
}