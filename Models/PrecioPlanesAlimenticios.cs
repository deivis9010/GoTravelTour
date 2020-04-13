using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioPlanesAlimenticios
    {
        public int PrecioPlanesAlimenticiosId { get; set; }       
        public decimal Precio { get; set; }
        public int PlanesAlimenticiosId { get; set; }
        public PlanesAlimenticios PlanesAlimenticios { get; set; }
        
        public Contrato ContratoDelPrecio { get; set; }
        //public int TemporadaId { get; set; }
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Hotel { get; set; }
        //public Temporada Temporada { get; set; }
    }
}
