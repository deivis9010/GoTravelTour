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
        public List<ModificadorTemporada> ListaModificadoresActivos { get; set; }
        [NotMapped]
        public List<Restricciones> ListaRestricciones { get; set; }
        [NotMapped]
        public List<PrecioRentaAutos> ListaPrecioAutos { get; set; }
        [NotMapped]
        public List<PrecioServicio> ListaPrecioServicioActividad { get; set; }
        [NotMapped]
        public List<PrecioTraslado> ListaPrecioTraslados { get; set; }
        [NotMapped]
        public List<ModificadorProductos> ListaModificadores { get; set; }
        [NotMapped]
        public List<PrecioPlanesAlimenticios> ListaPrecioPlanes { get; set; }
        [NotMapped]
        public List<PrecioAlojamiento> ListaPrecioAlojamientos { get; set; }





    }
}
