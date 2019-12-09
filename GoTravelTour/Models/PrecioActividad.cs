using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PrecioActividad
    {

        public int PrecioActividadId { get; set; }
        public int HorasAdicionales { get; set; }
        public double Incluido { get; set; }
        public double PrecioAdulto { get; set; }
        public double PrecioNino { get; set; }
        public double PrecioInfante { get; set; }
        //public bool IsActivo { get; set; }        
        //public int ContratoId { get; set; }
       // public int TemporadaId { get; set; }
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Actividad { get; set; }
        public Contrato Contrato { get; set; }
        public Temporada Temporada { get; set; }
    }
}
