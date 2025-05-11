namespace Football.Domain.Entities
{
    public class Weather: BaseEntity
    {
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;

        public double Temperature { get; set; }
        public double RainChance { get; set; }
        public double WindSpeed { get; set; }
        public string Icon { get; set; } = string.Empty;
        public WeatherStatus Status { get; set; }
    }
}