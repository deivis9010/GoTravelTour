using GoTravelTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Utiles
{
    public class BuscadorActividad
    {
        public Cliente Cliente { get; set; }        
        public string NombreActividad { get; set; }       
        public Region RegionActividad { get; set; }
        public int CantidadAdultos { get; set; }
        public int CantidadMenores { get; set; }
        public int CantidadInfantes { get; set; }
        public DateTime Fecha { get; set; }
    }
}
