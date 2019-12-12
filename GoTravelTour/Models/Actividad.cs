using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Actividad: Producto
    {
        public int ActividadId { get; set; }
        public string Modalidad { get; set; } //Grupal o no
        public int CantidadPersonas { get; set; }
        public int Duracion { get; set; }
        public int MaxDuracion { get; set; }
        public bool HasTransporte { get; set; }
        public List<Comodidades> Comodidades { get; set; }
    }
}
