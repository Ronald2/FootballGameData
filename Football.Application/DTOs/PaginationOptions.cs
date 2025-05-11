namespace Football.Application.DTOs
{
    /// <summary>
    /// Opciones de configuración para la paginación global de la API.
    /// </summary>
    public class PaginationOptions
    {
        /// <summary>
        /// Página por defecto.
        /// </summary>
        public int DefaultPage { get; set; } = 1;
        /// <summary>
        /// Tamaño de página por defecto.
        /// </summary>
        public int DefaultPageSize { get; set; } = 20;
        /// <summary>
        /// Tamaño máximo de página permitido.
        /// </summary>
        public int MaxPageSize { get; set; } = 100;
    }
}
