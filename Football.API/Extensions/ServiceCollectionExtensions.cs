using Football.Application.Interfaces;
using Football.Application.Services;
using Football.Domain.Interfaces;
using Football.Infrastructure.Repositories;

namespace Football.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFootballServices(this IServiceCollection services)
        {
            services.AddScoped<ITeamRepository, EfTeamRepository>();
            services.AddScoped<IPlayerRepository, EfPlayerRepository>();
            services.AddScoped<IGameRepository, EfGameRepository>();
            services.AddScoped<ILineUpRepository, EfLineUpRepository>();
            services.AddScoped<IWeatherRepository, EfWeatherRepository>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<ILineUpService, LineUpService>();
            return services;
        }
    }
}
