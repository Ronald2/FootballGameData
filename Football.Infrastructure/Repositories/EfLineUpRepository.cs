using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Football.Domain.Entities;
using Football.Domain.Interfaces;
using Football.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Football.Infrastructure.Repositories
{
    public class EfLineUpRepository : ILineUpRepository
    {
        private readonly FootballDbContext _context;

        public EfLineUpRepository(FootballDbContext context)=> _context = context;
        public async Task AddAsync(LineUp lineUp)
        {
            await _context.LineUps.AddAsync(lineUp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(LineUp lineUp)
        {
            _context.LineUps.Remove(lineUp);
            await _context.SaveChangesAsync();
        }

        public async Task<LineUp?> GetByIdAsync(int id)
        {
            return await _context.LineUps
                .Include(l => l.Player)
                .Include(l => l.Team)
                .Include(l => l.Game)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IReadOnlyList<LineUp>> ListByGameAsync(int gameId)
        {
            return await _context.LineUps
                .AsNoTracking()
                .Where(l => l.GameId == gameId)
                .Include(l => l.Player)
                .Include(l => l.Team)
                .ToListAsync();
        }

        public async Task UpdateAsync(LineUp lineUp)
        {
            _context.LineUps.Update(lineUp);
            await _context.SaveChangesAsync();
        }

        public async Task<(IReadOnlyList<LineUp> Items, int TotalCount)> GetPagedByGameAsync(int gameId, int page, int pageSize)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be greater than 0.");

            var query = _context.LineUps
                                .AsNoTracking()
                                .Where(l => l.GameId == gameId)
                                .Include(l => l.Player)
                                .Include(l => l.Team);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}