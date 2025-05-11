namespace Football.Domain.Entities
{
    public class Game : BaseEntity
    {
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;

        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;

        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;

        public ICollection<LineUp> LineUps { get; set; } = new List<LineUp>();
        public Weather? Weather { get; set; }
    }
}
