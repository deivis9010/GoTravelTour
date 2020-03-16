using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ModificadorProductos
    {
        public int ModificadorProductosId { get; set; }
        public int ModificadorId { get; set; }    
        public int ProductoId { get; set; }
        [ForeignKey("ModificadorId")]
        public Modificador Modificador { get; set; }
        [ForeignKey ("ProductoId")]
        public Producto Producto { get; set; }
        
    }
}
