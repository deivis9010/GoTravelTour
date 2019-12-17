using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PuntoInteres
    {
        public int PuntoInteresId { get; set; }
        public string Nombre { get; set; }
        public int RegionId { get; set; }
        [JsonIgnore]
        public Region Region { get; set; }
    }
}
