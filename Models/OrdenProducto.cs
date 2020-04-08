using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenProducto
    {
        public int OrdenProductoId { get; set; }
        public int ProductoId { get; set; }
        public int OrdenId { get; set; }
        public Producto Producto { get; set; }
        public Orden Orden { get; set; }

    }
}
