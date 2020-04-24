using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioComodidades
    {
        public int PrecioComodidadesId { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Precio { get; set; }
        public int ComodidadesId { get; set; }
        public int ProductoId { get; set; }
        public Comodidades Comodidad { get; set; }
        public Producto Producto { get; set; }
    }
}
