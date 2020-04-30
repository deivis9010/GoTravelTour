using GoTravelTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Utiles
{
    public class BuscadorTraslado
    {
        public Cliente Cliente { get; set; }
        public int CantidadPasajeros { get; set; }
        //public string TipoTraslado { get; set; }
        public PuntoInteres Origen { get; set; }
        public PuntoInteres Destino { get; set; }
        public DateTime Fecha { get; set; }
    }
}
