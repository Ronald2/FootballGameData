namespace Football.Domain.Interfaces
{
    using Football.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITeamRepository
    {
        Task<Team?> GetByIdAsync(int id);
        Task<IReadOnlyList<Team>> ListAllAsync();
        Task AddAsync(Team team);
        Task UpdateAsync(Team team);
        Task DeleteAsync(Team team);
        Task<(IReadOnlyList<Team> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? nameFilter = null,
        string? coachFilter = null);
    }
}