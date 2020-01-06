using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Temporada
    {
        public int TemporadaId { get; set; }
        public string Nombre { get; set; }
        public string DiasSemana { get; set; }
        public int ContratoId { get; set; }
        public bool PermiteHacerCopia { get; set; }
        public Contrato Contrato { get; set; }
        public List<RangoFechas> ListaFechasTemporada { get; set; }
        [NotMapped]
        public List<RestriccionesActividad> RestriccionesActividads { get; set; }
        [NotMapped]
        public List<RestriccionesRentasAutos> RestriccionesRentasAutos { get; set; }




    }
}
