using System.Net.Http.Json;
using Football.Application.DTOs;
using Football.Application.Mapping;
using Football.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Football.Tests.Integration
{
    public class ApiIntegrationTests
    {
        private WebApplicationFactory<Football.API.Program> CreateFactory(string dbName)
        {
            return new WebApplicationFactory<Football.API.Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Eliminar todos los servicios relacionados con DbContext y proveedores
                        var dbContextDescriptors = services.Where(d => d.ServiceType.FullName != null &&
                            (d.ServiceType.FullName.Contains("DbContextOptions") ||
                             d.ImplementationType?.FullName?.Contains("SqlServer") == true ||
                             d.ImplementationType?.FullName?.Contains("InMemory") == true)).ToList();
                        foreach (var desc in dbContextDescriptors)
                            services.Remove(desc);

                        // Register InMemory DbContext with unique db name
                        services.AddDbContext<FootballDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(dbName);
                        });

                        services.AddAutoMapper(typeof(MappingProfile).Assembly);
                    });
                });
        }

        [Fact]
        public async Task CreateAndGetTeam_ReturnsCorrectData()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactory(dbName);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Environment", "Testing");

            var newTeam = new TeamDto
            {
                Tricode = "TST",
                Name = "Test FC",
                Coach = "Tester"
            };

            var postResponse = await client.PostAsJsonAsync("/api/teams", newTeam);
            if (!postResponse.IsSuccessStatusCode)
            {
                var errorContent = await postResponse.Content.ReadAsStringAsync();
                throw new Exception($"POST failed: {postResponse.StatusCode}\n{errorContent}");
            }

            var created = await postResponse.Content.ReadFromJsonAsync<TeamDto>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0, "Id should be greater than 0");
            Assert.Equal(newTeam.Tricode, created.Tricode);

            var getResponse = await client.GetAsync($"/api/teams/{created.Id}");
            Assert.True(getResponse.IsSuccessStatusCode, $"GET failed: {getResponse.StatusCode}");

            var fetched = await getResponse.Content.ReadFromJsonAsync<TeamDto>();
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);
            Assert.Equal("Test FC", fetched.Name);
            Assert.Equal("Tester", fetched.Coach);
        }

        [Fact]
        public async Task ListTeams_Pagination_WorksCorrectly()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactory(dbName);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Environment", "Testing");

            // Arrange: crear múltiples equipos
            for (int i = 1; i <= 15; i++)
            {
                var response = await client.PostAsJsonAsync("/api/teams", new TeamDto { Tricode = $"T{i}", Name = $"Team {i}", Coach = $"Coach {i}" });
                Assert.True(response.IsSuccessStatusCode, $"POST failed for T{i}: {response.StatusCode}");
            }

            // Act: listar primera página pageSize=10
            var responsePage1 = await client.GetAsync("/api/teams?page=1&pageSize=10");
            Assert.True(responsePage1.IsSuccessStatusCode, $"GET page 1 failed: {responsePage1.StatusCode}");
            var page1 = await responsePage1.Content.ReadFromJsonAsync<PagedResultDto<TeamDto>>();

            Assert.NotNull(page1);
            Assert.Equal(10, page1.Items.Count());
            Assert.Equal(15, page1.TotalCount);
            Assert.Equal(1, page1.Page);
            Assert.Equal(10, page1.PageSize);

            // Act: segunda página
            var responsePage2 = await client.GetAsync("/api/teams?page=2&pageSize=10");
            Assert.True(responsePage2.IsSuccessStatusCode, $"GET page 2 failed: {responsePage2.StatusCode}");
            var page2 = await responsePage2.Content.ReadFromJsonAsync<PagedResultDto<TeamDto>>();

            Assert.NotNull(page2);
            Assert.Equal(5, page2.Items.Count());
            Assert.Equal(15, page2.TotalCount);
            Assert.Equal(2, page2.Page);
        }
    }
}
