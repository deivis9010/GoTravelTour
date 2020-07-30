using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenVehiculo
    {
        public int OrdenVehiculoId { get; set; }
        public string NumeroConfirmacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaRecogida { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int OrdenId { get; set; }
        public Orden Orden { get; set; }
        public PuntoInteres LugarRecogida { get; set; }
        public PuntoInteres LugarEntrega { get; set; }
        public int DistribuidorId { get; set; }
        public string TelefonoReferencia { get; set; }
        public string DireccionReferencia { get; set; }
        public ConfiguracionVoucher Voucher { get; set; }
        public string InformacionLlegada { get; set; }
        public string InformacionSalida { get; set; }
        public List<OrdenVehiculoPrecioRentaAuto> ListaPreciosRentaAutos { get; set; }
        public Distribuidor Distribuidor { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public bool PremiteCopia { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal PrecioOrden { get; set; }

        public Sobreprecio Sobreprecio { get; set; }
        public decimal ValorSobreprecioAplicado { get; set; }


    }
}
