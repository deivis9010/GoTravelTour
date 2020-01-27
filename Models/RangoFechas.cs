using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class RangoFechas
    {
        public int RangoFechasId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TemporadaId { get; set; }
        public Temporada Temporada { get; set; }


    }
}
