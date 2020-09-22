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
        public string TipoTraslado { get; set; }
        public int CantidadAdultTras { get; set; }
        public int CantidadInfantesTras { get; set; }
        public int CantidadNinoTras { get; set; }
        public int TipoTransporteId { get; set; }
        public int IdQB { get; set; }
        public TipoTransporte TipoTransporte { get; set; }


    }
}
