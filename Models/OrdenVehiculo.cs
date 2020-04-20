using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenVehiculo
    {
        public int OrdenVehiculoId { get; set; }
        public string NumeroConfirmacion { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaRecogida { get; set; }
        public DateTime FechaEntrega { get; set; }
        public PuntoInteres LugarRecogida { get; set; }
        public PuntoInteres LugarEntrega { get; set; }
        public int DistribuidorId { get; set; }
        public PrecioRentaAutos PrecioRentaAutos { get; set; }
        public Distribuidor Distribuidor { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public bool PremiteCopia { get; set; }


    }
}
