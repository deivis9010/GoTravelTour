using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class HabitacionTipoHabitacion
    {
        public int HabitacionTipoHabitacionId { get; set; }
        public int HabitacionId { get; set; }
        public int TipoHabitacionId { get; set; }
        public Habitacion Habitacion { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }
    }
}
