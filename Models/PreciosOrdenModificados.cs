using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PreciosOrdenModificados
    {
        public int PreciosOrdenModificadosId { get; set; }
        public int OrdenId { get; set; }
        public int OrdenVehiculoId { get; set; }
        public int OrdenTrasladoId { get; set; }
        public int OrdenAlojamientoId { set; get; }
        public int OrdenActividadId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Precio { get; set; }
        public string TipoPersona { get; set; }

    }
}
