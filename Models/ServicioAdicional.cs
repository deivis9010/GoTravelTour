using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ServicioAdicional : Producto
    {
        public int ServicioAdicionalId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string TipoViajeBoleto { get; set; }
        public int TipoServicioAdicionalId { get; set; }
        public TipoServicioAdicional TipoServicioAdicional { get; set; }
    }
}
