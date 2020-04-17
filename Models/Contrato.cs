using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Contrato
    {
        public int ContratoId { get; set; }
        public string Nombre { get; set; }
        public bool IsActivo { get; set; }
        public bool OfertaEspecial { get; set; }
        public int FormaCobro { get; set; } // 1-por dia 2- PrimeraTemp 3-UltimaTemp
        public string CodigoOfertaEspecial { get; set; }       
        public DateTime? FechaInicioTravel { get; set; }
        public DateTime? FechaFinTravel { get; set; }
        public DateTime? FechaInicioBooking { get; set; }
        public DateTime? FechaFinBooking { get; set; }
        public bool PermiteHacerCopia { get; set; }
        public int TipoProductoId { get; set; }
        public int DistribuidorId { get; set; }
        public Distribuidor Distribuidor { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public List<Temporada> Temporadas { get; set; }
        [NotMapped]
        public List<NombreTemporada> NombreTemporadas { get; set; }
       


    }
}
