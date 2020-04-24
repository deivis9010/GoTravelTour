using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenActividad
    {
        public int OrdenActividadId { get; set; }
        public DateTime FechaActividad { get; set; }
        public int? CantAdulto { get; set; }
        public int? CantNino { get; set; }
        public int? CantInfante { get; set; }
        public string TelefonoReferencia { get; set; }
        public string DireccionReferencia { get; set; }
        public string NombreCliente { get; set; }
        public string VenueName { get; set; } //que es
        public string NumeroConfirmacion { get; set; }
        public string DescripcionServicio { get; set; }
        public string NotasAdicionales { get; set; }
        public int DistribuidorId { get; set; }
        public Distribuidor Distribuidor { get; set; }
        public int ActividadId { get; set; }
        public Actividad Actividad { get; set; }
        public PrecioActividad PrecioActividad { get; set; }
       // public List<PrecioServicio> Servicios { get; set; }
        public bool PremiteCopia { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioOrden { get; set; }

    }
}
