using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Vehiculo: Producto
    {
        public int VehiculoId { get; set; }
        public string TipoTransmision { get; set; }
        public int CantidadPlazas { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }


    }
}
