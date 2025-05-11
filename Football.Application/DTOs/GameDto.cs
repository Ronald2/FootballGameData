namespace Football.Application.DTOs
{
    public class GameDto
    {
         public int Id { get; set; }
        public DateTime Date { get; set; }
        public TeamDto HomeTeam { get; set; } = new TeamDto();

        public TeamDto AwayTeam { get; set; } = new TeamDto();

        public IEnumerable<LineUpDto> LineUps { get; set; } = new List<LineUpDto>();

        public WeatherDto? Weather { get; set; }
    }
}