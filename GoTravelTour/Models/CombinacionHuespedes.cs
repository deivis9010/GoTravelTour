using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class CombinacionHuespedes
    {
        public int CombinacionHuespedesId { get; set; }
        public int CantInfantes { get; set; }
        public int CantNino { get; set; }
        public int CantAdult { get; set; }
        public bool IsActivo { get; set; }
        public Producto Hotel { get; set; }
        public Habitacion Habitacion { get; set; }

    }
}
