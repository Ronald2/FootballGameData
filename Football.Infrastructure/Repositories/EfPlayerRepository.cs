using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Football.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Football.Infrastructure.Repositories
{
    public class EfPlayerRepository : IPlayerRepository
    {
        private readonly FootballDbContext _context;
        public EfPlayerRepository(FootballDbContext context) => _context = context;

        public async Task AddAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Player player)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Player>> ListByTeamAsync(int teamId)
        {
            return await _context.Players
                .AsNoTracking()
                .Where(p => p.TeamId == teamId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
        }

        public async Task<(IReadOnlyList<Player> Items, int TotalCount)> GetPagedByTeamAsync(
            int teamId,
            int page,
            int pageSize)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be greater than 0.");

            var query = _context.Players
                                .AsNoTracking()
                                .Where(p => p.TeamId == teamId);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

    }
}