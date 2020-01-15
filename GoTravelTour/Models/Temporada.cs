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
        public List<Restricciones> ListaRestricciones { get; set; }
        [NotMapped]
        public List<PrecioRentaAutos> ListaPrecioAutos { get; set; }
        [NotMapped]
        public List<PrecioActividad> ListaPrecioActividad { get; set; }




    }
}
