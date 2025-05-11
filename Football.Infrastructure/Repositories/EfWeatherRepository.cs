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
    public class EfWeatherRepository : IWeatherRepository
    {
        private readonly FootballDbContext _context;
        public EfWeatherRepository(FootballDbContext context) => _context = context;


        public async Task AddAsync(Weather weather)
        {
            await _context.Weathers.AddAsync(weather);
            await _context.SaveChangesAsync();
        }

        public async Task<Weather?> GetByGameIdAsync(int gameId)
        {
            return await _context.Weathers
                .AsNoTracking()
                .Include(w => w.Game)
                .FirstOrDefaultAsync(w => w.GameId == gameId);
        }

        public async Task UpdateAsync(Weather weather)
        {
            _context.Weathers.Update(weather);
            await _context.SaveChangesAsync();
        }

        public async Task<(IReadOnlyList<Weather> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be greater than 0.");

            var query = _context.Weathers
                .AsNoTracking()
                .Include(w => w.Game);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}