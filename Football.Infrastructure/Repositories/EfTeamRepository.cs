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
    public class EfTeamRepository : ITeamRepository
    {
        private readonly FootballDbContext _context;
        public EfTeamRepository(FootballDbContext context) => _context = context;

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Team team)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.Players)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IReadOnlyList<Team>> ListAllAsync()
        {
            return await _context.Teams
            .AsNoTracking()
            .Include(t => t.Players)
            .ToListAsync();
        }

        public async Task UpdateAsync(Team team)
        {
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
        }

        public async Task<(IReadOnlyList<Team> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? nameFilter = null,
            string? coachFilter = null)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be greater than 0.");

            var query = _context.Teams
                                .AsNoTracking()
                                .Include(t => t.Players)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(t => t.Name.Contains(nameFilter));

            if (!string.IsNullOrWhiteSpace(coachFilter))
                query = query.Where(t => t.Coach.Contains(coachFilter));

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}