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
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Incluido { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? PrecioAdulto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? PrecioNino { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? PrecioInfante { get; set; }
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public Temporada Temporada { get; set; }
    }
}
