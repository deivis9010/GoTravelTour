using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioActividad
    {
        public int PrecioActividadId { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioAdulto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioNino { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioInfante { get; set; }
        [ForeignKey("ProdcutoId")]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public Temporada Temporada { get; set; }
    }
}
