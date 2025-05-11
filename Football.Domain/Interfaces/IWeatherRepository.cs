namespace Football.Domain.Interfaces
{
    using Football.Domain.Entities;
    using System.Threading.Tasks;

    public interface IWeatherRepository
    {
        Task<Weather?> GetByGameIdAsync(int gameId);
        Task AddAsync(Weather weather);
        Task UpdateAsync(Weather weather);
    }
}