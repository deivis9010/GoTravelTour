using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Orden
    {
        public int OrdenId { get; set; }
        public string Estado { get; set; }
        public string NumeroOrden { get; set; }
        public int Duracion { get; set; }
        public string Nombre { get; set; }
        public int ClienteId { get; set; }
        public bool OFACrequired { get; set; }
        public bool HasVoucher { get; set; }
        public string Notas { get; set; }
        public string IntercomConferceNumber { get; set; }
        public Cliente Cliente { get; set; }
        public Usuario Creador { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public Sobreprecio Sobreprecio { get; set; }
        public List<OrdenProducto> ListaProductosEnOrden { get; set; }
    }
}
