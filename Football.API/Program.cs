using Football.Application.DTOs;
using Football.API.Extensions;
using Football.API.Middleware;
using Football.Application.Mapping;
using Football.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de paginación centralizada
builder.Services.Configure<PaginationOptions>(
    builder.Configuration.GetSection("Pagination"));

// Add FluentValidation services
builder.Services.AddFluentValidationServices();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<FootballDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Mover registro de servicios a extensión
builder.Services.AddFootballServices();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace Football.API
{
    public partial class Program { }
}