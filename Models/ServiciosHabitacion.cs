using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ServiciosHabitacion
    {
        public int ServiciosHabitacionId { get; set; }
        public string Nombre { get; set; }
        public List<HabitacionServiciosHabitacion> ListaServiciosHabitacion { get; set; }
    }
}
