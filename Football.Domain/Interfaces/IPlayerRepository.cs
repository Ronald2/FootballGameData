namespace Football.Domain.Interfaces
{
    using Football.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPlayerRepository
    {
        Task<Player?> GetByIdAsync(int id);
        Task<IReadOnlyList<Player>> ListByTeamAsync(int teamId);
        Task AddAsync(Player player);
        Task UpdateAsync(Player player);
        Task DeleteAsync(Player player);
        Task<(IReadOnlyList<Player> Items, int TotalCount)> GetPagedByTeamAsync(
            int teamId,
            int page,
            int pageSize);
    }
}