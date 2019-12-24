using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ContratoProducto
    {
        public int ContratoProductoId { get; set; }
      
        public Contrato Contrato { get; set; }
        public Producto Producto { get; set; }

    }
}
