using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Region
    {
        public int RegionId { get; set; }
        public string Nombre { get; set; }
        public List<PuntoInteres> PuntosDeInteres { get; set; }
    }
}
