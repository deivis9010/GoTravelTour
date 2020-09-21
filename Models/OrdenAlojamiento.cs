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
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public int IdBillQB { get; set; } //id del estimado creado en cQB  para poder editarlo si esta en null es  pq a la orden no se le ha creado el estimado
        public int OrdenId { get; set; }
        public Orden Orden { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public ConfiguracionVoucher Voucher { get; set; }
        //Congiguracion elegida
        public List<OrdenAlojamientoPrecioAlojamiento> ListaPrecioAlojamientos { get; set; }

        public Habitacion Habitacion { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }
        public int DistribuidorId { get; set; }
        public Distribuidor Distribuidor { get; set; }
        public int PlanesAlimenticiosId { get; set; }
        public PlanesAlimenticios PlanAlimenticio { get; set; }        
        public PrecioPlanesAlimenticios PrecioPlanesAlimenticios { get; set; }
        public int AlojamientoId { get; set; }
        public Alojamiento Alojamiento { set; get; }
        public Modificador ModificadorAplicado { set; get; }
        //FIN----------------
        public bool PremiteCopia { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioOrden { get; set; }

        public Sobreprecio Sobreprecio { get; set; }
        public decimal ValorSobreprecioAplicado { get; set; }

    }
}
