using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Habitacion
    {
        public int HabitacionId { get; set; }
        public string Nombre { get; set; }
        public string SKU { get; set; }
        public string Descripcion { get; set; }
        public bool IsActiva { get; set; }
        public bool IsPayPerRoom { get; set; }
        public int TipoHabitacionId { get; set; }
        public int ProductoId { get; set; }
        public bool PermiteHacerCopia { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }
        public Producto Producto { get; set; }
        public List<CombinacionHuespedes> ListaCombinacionesDisponibles { get; set; }
        public List<HabitacionServiciosHabitacion> ListaServiciosHabitacion { get; set; }
    }
}
