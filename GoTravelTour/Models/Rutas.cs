using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Rutas
    {
        public int RutasId { get; set; }
        public Region regionOrigen { get; set; }
        public Region regionDestino { get; set; }
        public PuntoInteres puntoOrigen { get; set; }
        public PuntoInteres puntoDestino { get; set; }

    }
}
