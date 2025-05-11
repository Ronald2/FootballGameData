using Football.Domain.Entities;

namespace Football.Application.DTOs
{
    public class LineUpDto
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public PlayerDto Player { get; set; } = new PlayerDto();
        public string Position { get; set; } = string.Empty;
        public string Spot { get; set; } = string.Empty;
        public LineUpStatus Status { get; set; }
    }
}