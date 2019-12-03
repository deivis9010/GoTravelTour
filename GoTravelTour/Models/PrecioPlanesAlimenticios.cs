using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioPlanesAlimenticios
    {
        public int PrecioPlanesAlimenticiosId { get; set; }       
        public double Precio { get; set; }        
        public Producto Hotel { get; set; }
        public Contrato Contrato { get; set; }
        public Temporada Temporada { get; set; }
    }
}
