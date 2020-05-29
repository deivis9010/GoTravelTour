using GoTravelTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Utiles
{
    public class BuscadorAlojamiento
    {
       
        public string NombreHotel { get; set; }
        public TipoAlojamiento TipoAlojamiento { get; set; }
        public Region Region { get; set; }
       // public string NombreHabitacion { get; set; }       
        public bool OrdenarAsc { get; set; }
        public int CantidadEstrellas { get; set; }
        public bool IsPasaDia { get; set; }
        public int CantidadNoches { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Salida { get; set; }

    }
}
