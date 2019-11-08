using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class UsuarioInfo
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }       
        public string Email { get; set; }
        public string Rol { get; set; }
        public DateTime ValidoDesde { get; set; }
        public DateTime ValidoHasta { get; set; }
    }
}
