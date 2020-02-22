using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioAlojamiento
    {
        public int PrecioAlojamientoId { get; set; }
        public decimal Precio { get; set; }
        
        public int ProductoId { get; set; }
        
        [ForeignKey("ProductoId")]
        public Producto Hotel { get; set; }
        public Contrato Contrato { get; set; }
        public Temporada Temporada { get; set; }
        public Habitacion Habitacion { get; set; }
        //public List<Modificador> Modificadores { get; set; }
    }
}
