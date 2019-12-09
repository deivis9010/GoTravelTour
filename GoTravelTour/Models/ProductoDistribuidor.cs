using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ProductoDistribuidor
    {
        public int ProductoDistribuidorId { get; set; }
        public int ProductoId { get; set; }
        public int DistribuidorId { get; set; }
        public Producto Producto { get; set; }
        public Distribuidor Distribuidor { get; set; }

    }
}
