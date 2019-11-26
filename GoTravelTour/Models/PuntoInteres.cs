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
        [JsonIgnore]
        public Region region { get; set; }
    }
}
