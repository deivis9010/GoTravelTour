using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioTraslado
    {
        public int PrecioTrasladoId { get; set; }
        public double Precio { get; set; }
        //public bool IsActivo { get; set; }
        //public int ContratoId { get; set; }
        //public int TemporadaId { get; set; }
        public int ProductoId { get; set; }
        public int RutasId { get; set; }        
        [ForeignKey("ProductoId")]
        public Producto Traslado { get; set; }
        public Contrato Contrato { get; set; }
        public Temporada Temporada { get; set; }
        public Rutas Rutas { get; set; }

    }
}
