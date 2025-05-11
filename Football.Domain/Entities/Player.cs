namespace Football.Domain.Entities
{
    /// <summary>
    /// Representa un jugador de fútbol.
    /// </summary>
    public class Player : BaseEntity
    {
        /// <summary>
        /// Identificador del equipo al que pertenece el jugador.
        /// </summary>
        public int TeamId { get; set; }
        /// <summary>
        /// Equipo al que pertenece el jugador.
        /// </summary>
        public Team? Team { get; set; }
        /// <summary>
        /// Número del jugador en el equipo.
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Nombre del jugador.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;
        /// <summary>
        /// Apellido del jugador.
        /// </summary>
        public string LastName { get; set; } = string.Empty;
        /// <summary>
        /// Nombre completo del jugador.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";
    }
}