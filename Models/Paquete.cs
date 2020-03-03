using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Paquete
    {
        public int PaqueteId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ImageContentP { get; set; }
        public string TipoImagenP { get; set; }
        public string ImageContent1 { get; set; }
        public string TipoImagen1 { get; set; }
        public string ImageContent2 { get; set; }
        public string TipoImagen2 { get; set; }
    }
}
