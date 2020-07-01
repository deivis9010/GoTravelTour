using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Utiles
{
    public class BuscadorOrden
    {
        public string Nombre { get; set; }
        public string NumeroOrden { get; set; }
        public int ProveedorId { get; set; }
        public List<string> Estados { get; set; }
        public DateTime? FechaI { get; set; }
        public DateTime? FechaF { get; set; }

        public string col { get; set; }
        public string filter { get; set; }
        public string sortDirection { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
      
        
    }
}
