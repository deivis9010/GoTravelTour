using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Alojamiento: Producto
    {
        public int AlojamientoId { get; set; }
        public int Categoria { get; set; }
        public int TipoAlojamientoId { get; set; }
        public TipoAlojamiento TipoAlojamiento { get; set; }


    }
}
