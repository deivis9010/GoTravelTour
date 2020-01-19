using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Sobreprecio
    {
        public int SobreprecioId { get; set; }
        public decimal PrecioDesde { get; set; }
        public decimal PrecioHasta { get; set; }
        public decimal ValorPorCiento { get; set; }
        public decimal ValorDinero { get; set; }
        public bool PagoPorDia { get; set; }
        public int TipoProductoId { get; set; }
        public TipoProducto TipoProducto { get; set; }




    }
}
