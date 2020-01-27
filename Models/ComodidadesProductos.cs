using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ComodidadesProductos
    {
        public int ComodidadesProductosId { get; set; }
        public int ProductoId { get; set; }
        public int ComodidadesId { get; set; }
        public Producto Producto { get; set; }
        public Comodidades Comodidades { get; set; }
    }
}
