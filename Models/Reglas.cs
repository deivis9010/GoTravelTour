using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Reglas
    {
        public int ReglasId { get; set; }
        public string TipoPrecioHabitacion { get; set; }
        public string TipoPersona { get; set; }
        public double PrecioFijo { get; set; }
        public double PrecioPorCiento { get; set; }
        public bool IsActivo { get; set; }
        public int ModificadorId { get; set; }
        public int TipoHabitacionId { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }
        public Modificador Modificador { get; set; }

    }
}
