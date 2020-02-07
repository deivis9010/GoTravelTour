using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioServicio
    {
        public int PrecioServicioId { get; set; }
        public int HorasAdicionales { get; set; }
        public decimal? Incluido { get; set; }
        public decimal? PrecioAdulto { get; set; }
        public decimal? PrecioNino { get; set; }
        public decimal? PrecioInfante { get; set; }
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public Temporada Temporada { get; set; }
    }
}
