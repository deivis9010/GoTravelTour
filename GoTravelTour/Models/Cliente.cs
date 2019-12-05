using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; }
        public string Localizador { get; set; } // (id generado por quickbooks)
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string Calle { get; set; }
        public string ZipCode { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string TipoTrasaccion { get; set; }
        public string ImageContent { get; set; } //Para el logo
        public string ImageMimeType { get; set; } //Para el logo
        public string ImageName { get; set; } //Para el logo
        public double? Descuento { get; set; }
        public bool IsActivo { get; set; }
        public bool IsPublic { get; set; }
        public string Pais { get; set; }
		//public List<Usuario> ListaUsuarios { get; set; }


    }
}
