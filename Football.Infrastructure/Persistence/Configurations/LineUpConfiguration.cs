using Football.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Football.Infrastructure.Persistence.Configurations
{
    public class LineUpConfiguration : IEntityTypeConfiguration<LineUp>
    {
        public void Configure(EntityTypeBuilder<LineUp> builder)
        {
            builder.HasOne(l => l.Game)
                   .WithMany(g => g.LineUps)
                   .HasForeignKey(l => l.GameId);

            builder.HasOne(l => l.Team)
                   .WithMany()
                   .HasForeignKey(l => l.TeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Player)
                   .WithMany()
                   .HasForeignKey(l => l.PlayerId);
        }
    }
}
