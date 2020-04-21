using System;
using System.Collections.Generic;
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
        public string NumeroConfirmacion { get; set; }
        public string InformacionLlegada { get; set; }
        public string NombreCliente { get; set; }
        public string InformacionSalida { get; set; }
        public string DescripcionServicio { get; set; }

        public PrecioTraslado PrecioTraslado { get; set; }  
        public ConfiguracionVoucher ConfiguracionVoucher { set; get; }
        public string TipoTraslado { get; set; } //duda que cosa es y como se gestiona
        public int DistribuidorId { get; set; }
        public Distribuidor Distribuidor { get; set; }       
        public PuntoInteres LugarRecogida { get; set; }
        public PuntoInteres LugarEntrega { get; set; }    
        public Traslado Traslado { get; set; }
        public bool PremiteCopia { get; set; }
        public decimal PrecioOrden { get; set; }

    }
}
