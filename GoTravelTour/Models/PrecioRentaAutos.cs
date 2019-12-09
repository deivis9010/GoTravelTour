using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioRentaAutos
    {
        public int PrecioRentaAutosId { get; set; }
        public int DiasExtra { get; set; }
        public double Seguro { get; set; }
        public double Precio { get; set; }
        //public bool IsActivo { get; set; }
       //public int ContratoId { get; set; }
        //public int TemporadaId { get; set; }
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Auto { get; set; }
        public Contrato Contrato { get; set; }
        public Temporada Temporada { get; set; }
    }

}
