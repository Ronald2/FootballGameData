using Football.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Football.Infrastructure.Persistence.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasOne(g => g.HomeTeam)
                   .WithMany()
                   .HasForeignKey(g => g.HomeTeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.AwayTeam)
                   .WithMany()
                   .HasForeignKey(g => g.AwayTeamId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
