using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenAlojamiento
    {
        public int OrdenAlojamientoId { set; get; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
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
        public int OrdenId { get; set; }
        public Orden Orden { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public ConfiguracionVoucher ConfiguracionVoucher { set; get; }
        //Congiguracion elegida
        public PrecioAlojamiento PrecioAlojamiento { get; set; }
       
        public Habitacion Habitacion { get; set; }
        public int DistribuidorId { get; set; }
        public Distribuidor Distribuidor { get; set; }
        public int PlanesAlimenticiosId { get; set; }
        public PlanesAlimenticios PlanAlimenticio { get; set; }
        public int AlojamientoId { get; set; }
        public Alojamiento Alojamiento { set; get; }
        //FIN----------------
        public bool PremiteCopia { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioOrden { get; set; }

        public Sobreprecio Sobreprecio { get; set; }
        public decimal ValorSobreprecioAplicado { get; set; }

    }
}
