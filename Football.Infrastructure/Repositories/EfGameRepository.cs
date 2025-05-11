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
    public class EfGameRepository : IGameRepository
    {
        private readonly FootballDbContext _context;

        public EfGameRepository(FootballDbContext context) => _context = context;


        public async Task AddAsync(Game game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Game game)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games
               .Include(g => g.HomeTeam)
               .Include(g => g.AwayTeam)
               .Include(g => g.LineUps)
               .Include(g => g.Weather)
               .FirstOrDefaultAsync(g => g.Id == id);

        }

        public async Task<IReadOnlyList<Game>> ListAllAsync()
        {
            return await _context.Games
                .AsNoTracking()
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .ToListAsync();
        }

        public async Task UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }

        public async Task<(IReadOnlyList<Game> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be greater than 0.");

            var query = _context.Games
                .AsNoTracking()
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}