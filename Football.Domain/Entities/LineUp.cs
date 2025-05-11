namespace Football.Domain.Entities
{
    public class LineUp : BaseEntity
    {
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        public string Position { get; set; } = string.Empty;
        public string Spot { get; set; } = string.Empty;

        public LineUpStatus Status { get; set; }

    }
}
