using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class OrdenServicioAdicional
    {
        public int OrdenServicioAdicionalId { get; set; }
        public int ServicioAdicionalId { get; set; }
        public ServicioAdicional ServicioAdicional { get; set; }
        public int CantidadAdultos { get; set; }
        public int CantidadNinos { get; set; }
        public int CantidadInfantes { get; set; }
        public decimal PrecioAdultos { get; set; }
        public decimal PrecioNinos { get; set; }
        public decimal PrecioInfantes { get; set; }       
        public string Descripcion { get; set; }
        public string TipoViaje { get; set; }
        public int OrdenId { get; set; }
        public Orden Orden { get; set; }
        public int DistribuidorId { get; set; }
        public Distribuidor Distribuidor { get; set; }
        public decimal PrecioOrden { get; set; }
        public int IdBillQB { get; set; } //id del estimado creado en cQB  para poder editarlo si esta en null es  pq a la orden no se le ha creado el estimado
    }
}
