using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ModificadorProductos
    {
        public int ModificadorProductosId { get; set; }
        //public int ModificadorId { get; set; }
        //public int ProductoId { get; set; }
        public Modificador Modificador { get; set; }
        public Producto Producto { get; set; }
        
    }
}
