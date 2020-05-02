using GoTravelTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Utiles
{
    public class BuscadorAlojamiento
    {
        public Cliente Cliente { get; set; }
        public string NombreHotel { get; set; }
        public TipoAlojamiento TipoAlojamiento { get; set; }
        public PuntoInteres Lugar { get; set; }
        public int CantidadAdultos { get; set; }
        public int CantidadMenores { get; set; }
        public bool IsPasaDia { get; set; }
        public int CantidadNoches { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Salida { get; set; }

    }
}
