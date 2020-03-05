using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class TipoHabitacion
    {
        public int TipoHabitacionId { get; set; }
        public string Nombre { get; set; }
        public List<HabitacionTipoHabitacion> ListaHabitaciones { get; set; }
    }
}
