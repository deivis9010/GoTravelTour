using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class CategoriaAuto
    {
        public int CategoriaAutoId { get; set; }
        public string Nombre { get; set; }
        public List<VehiculoCategoriaAuto> ListaVehiculos { get; set; }
    }
}
