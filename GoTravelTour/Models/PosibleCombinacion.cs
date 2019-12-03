using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PosibleCombinacion
    {
        public int PosibleCombinacionId { get; set; }
        public int CantInfantes { get; set; }
        public int CantNino { get; set; }
        public int CantAdult { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }

    }
}
