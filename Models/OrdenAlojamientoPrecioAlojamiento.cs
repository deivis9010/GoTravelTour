using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenAlojamientoPrecioAlojamiento
    {
        public int OrdenAlojamientoPrecioAlojamientoId { get; set; }
        public PrecioAlojamiento PrecioAlojamiento { get; set; }
        public OrdenAlojamiento OrdenAlojamiento { get; set; }
    }
}
