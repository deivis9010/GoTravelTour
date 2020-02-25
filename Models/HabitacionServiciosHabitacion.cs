using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class HabitacionServiciosHabitacion
    {
        public int HabitacionServiciosHabitacionId { get; set; }
        public int HabitacionId { get; set; }
        public int ServiciosHabitacionId { get; set; }
        public Habitacion Habitacion { get; set; }
        public ServiciosHabitacion ServiciosHabitacion { get; set; }
    }
}
