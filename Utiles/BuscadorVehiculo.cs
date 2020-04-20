using GoTravelTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Utiles
{
    public class BuscadorVehiculo
    {
        public BuscadorVehiculo()
        {

        }
        public Usuario Usuario { get; set; }
        public CategoriaAuto CategoriaAuto { get; set; }
        public string TipoTransmision { get; set; }
        public DateTime FechaRecogida { get; set; }
        public DateTime FechaEntrega { get; set; }


    }
}
