using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class AlojamientosPlanesAlimenticios
    {
        public int AlojamientosPlanesAlimenticiosId { get; set; }
        public int AlojamientoId { get; set; }
        public int PlanesAlimenticiosId { get; set; }
        public PlanesAlimenticios PlanesAlimenticios { get; set; }
        public Alojamiento Alojamiento { get; set; }

    }
}
