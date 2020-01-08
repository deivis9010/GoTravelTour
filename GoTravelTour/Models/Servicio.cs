using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Servicio
    {
        public int ServicioId { get; set; }
        public string Nombre { get; set; }
        public bool Opcional { get; set; }
        public string Categoria { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

    }
}
