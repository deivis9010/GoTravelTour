using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenVehiculoPrecioRentaAuto
    {
        public int OrdenVehiculoPrecioRentaAutoId { get; set; }
       
        public OrdenVehiculo OrdenVehiculo { get; set; }
        public PrecioRentaAutos PrecioRentaAutos { get; set; }
    }
}
