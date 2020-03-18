using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ModificadorTemporada
    {
        public int ModificadorTemporadaId { get; set; }
        public int ModificadorId { get; set; }
        public int TemporadaId { get; set; }
        public Modificador Modificador { get; set; }
        public Temporada Temporada { get; set; }
    }
}
