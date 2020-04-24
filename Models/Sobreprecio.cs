using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Sobreprecio
    {
        public int SobreprecioId { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioDesde { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioHasta { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? ValorPorCiento { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? ValorDinero { get; set; }
        public bool PagoPorDia { get; set; }
        public int TipoProductoId { get; set; }
        public TipoProducto TipoProducto { get; set; }




    }
}
