using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class AlmacenImagenes
    {
        public int AlmacenImagenesId { get; set; }
        public string NombreImagen { get; set; }
        public string Localizacion { get; set; }       //(Principal Galeria)
        public string ImageContent { get; set; }  //almacena la url
        public string TipoImagen { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }




    }
}
