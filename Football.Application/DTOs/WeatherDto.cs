namespace Football.Application.DTOs
{
    public class WeatherDto
    {
        public int GameId { get; set; }
        public double Temperature { get; set; }
        public double RainChance { get; set; }
        public double WindSpeed { get; set; }
        public string Icon { get; set; } = string.Empty;
    }
}