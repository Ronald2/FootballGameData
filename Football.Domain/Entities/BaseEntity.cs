using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Football.Domain.Entities
{
    /// <summary>
    /// Clase base para todas las entidades del dominio,
    /// incluyendo auditoría.
    /// </summary>
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}