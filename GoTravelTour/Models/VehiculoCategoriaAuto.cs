using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class VehiculoCategoriaAuto
    {
        public int VehiculoCategoriaAutoId { get; set; }
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
        public int CategoriaAutoId { get; set; }
        public CategoriaAuto CategoriaAuto { get; set; }
    }
}
