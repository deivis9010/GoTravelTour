using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Temporada
    {
        public int TemporadaId { get; set; }
        public string Nombre { get; set; }
        public Contrato Contrato { get; set; }
        public List<RangoFechas> ListaFechasTemporada { get; set; }



    }
}
