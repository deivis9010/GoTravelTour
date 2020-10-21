using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Proveedor //Owner
    {
        public int ProveedorId { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool IsActivo { get; set; }
        public int IdQB { get; set; }
        public string ImageContent { get; set; } //Para el logo url
        public string ImageMimeType { get; set; } //Para el logo
        public string ImageName { get; set; } //Para el logo
        public List<Producto> Productos { get; set; }

    }
}
