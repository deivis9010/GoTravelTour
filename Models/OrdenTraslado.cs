using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenTraslado
    {
        public int OrdenTrasladoId { get; set; }
        public int? CantAdulto { get; set; }
        public int? CantNino { get; set; }
        public int? CantInfante { get; set; }
        public DateTime FechaRecogida { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string NumeroConfirmacion { get; set; }
        public string InformacionLlegada { get; set; }
        public string NombreCliente { get; set; }
        public string InformacionSalida { get; set; }
        public string DescripcionServicio { get; set; }
        public bool IsIdaVuelta { get; set; }
        public string TelefonoReferencia { get; set; }
        public string DireccionReferencia { get; set; }
        public string TipoTrasladoOrden { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public int OrdenId { get; set; }
        public Orden Orden { get; set; }
        public ConfiguracionVoucher Voucher { get; set; }

        public PrecioTraslado PrecioTraslado { get; set; }  
        
        public string TipoTraslado { get; set; } //duda que cosa es y como se gestiona
        public int DistribuidorId { get; set; }
        public Distribuidor Distribuidor { get; set; }       
        public PuntoInteres PuntoOrigen { get; set; }
        public PuntoInteres PuntoDestino { get; set; }    
        public Traslado Traslado { get; set; }
        public bool PremiteCopia { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioOrden { get; set; }
        public Sobreprecio Sobreprecio { get; set; }
        public decimal ValorSobreprecioAplicado { get; set; }
    }
}
