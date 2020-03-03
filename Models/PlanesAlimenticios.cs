using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class PlanesAlimenticios
    {
        public int PlanesAlimenticiosId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public List<AlojamientosPlanesAlimenticios> ListaAlojamientos { get; set; }

    }
}
