using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Traslado : Producto
    {
        public int TrasladoId { get; set; }
        public int CapacidadTraslado { get; set; }
        public string ModeloTraslado { get; set; }


    }
}
