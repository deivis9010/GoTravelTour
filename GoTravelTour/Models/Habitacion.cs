using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Habitacion
    {
        public int HabitacionId { get; set; }        
        public string SKU { get; set; }
        public string Descripcion { get; set; }
        public bool IsActiva { get; set; }
        public bool IsPayPerRoom { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }
        public Producto Producto { get; set; }
    }
}
