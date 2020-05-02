using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        
        public string Username  { get; set; }
        public string Correo { get; set; }
        
        public string Password { get; set; }
        public string Telefono { get; set; }
        public bool IsActivo { get; set; }
        public int ClienteId { get; set; }
        public int RolId { get; set; }
        public Cliente cliente { get; set; }
        public Rol rol { get; set; }

    }
}
