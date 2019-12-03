using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class RestriccionesActividad
    {
        public int RestriccionesActividadId { get; set; }
        public int Minimo { get; set; }
        public int Maximo { get; set; }
        public double Precio { get; set; }
        public bool IsActivo { get; set; }
        public Producto Auto { get; set; }
        public Contrato Contrato { get; set; }
        public Temporada Temporada { get; set; }
    }
}
