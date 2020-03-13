using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class AlojamientosPlanesAlimenticios
    {
        public int AlojamientosPlanesAlimenticiosId { get; set; }
        
        public int ProductoId { get; set; }
        public int PlanesAlimenticiosId { get; set; }
        public PlanesAlimenticios PlanesAlimenticios { get; set; }
        public Producto Producto { get; set; }

    }
}
