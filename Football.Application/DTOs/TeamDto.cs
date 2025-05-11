namespace Football.Application.DTOs
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Tricode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Coach { get; set; } = string.Empty;
        public IEnumerable<PlayerDto> Players { get; set; } = new List<PlayerDto>();
    }
}