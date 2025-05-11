namespace Football.Domain.Interfaces
{
    using Football.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int id);
        Task<IReadOnlyList<Game>> ListAllAsync();
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(Game game);
        Task<(IReadOnlyList<Game> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    }
}