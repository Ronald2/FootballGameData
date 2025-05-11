using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Football.Domain.Entities
{
    public class Team : BaseEntity
    {
        public string Tricode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Coach { get; set; } = string.Empty;
        public ICollection<Player> Players { get; set; } = new List<Player>();
        
    }
}