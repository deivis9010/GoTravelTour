using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PosibleCombinacion
    {
        public int PosibleCombinacionId { get; set; }
        public string Nombre { get; set; }   //Para identificar una posible combinacion a la hora de asociarla a hotel
        public int CantInfantes { get; set; }
        public int CantNino { get; set; }
        public int CantAdult { get; set; }
        public int TipoHabitacionId { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }

    }
}
