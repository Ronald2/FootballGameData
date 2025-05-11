namespace Football.Application.DTOs
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

    }
}