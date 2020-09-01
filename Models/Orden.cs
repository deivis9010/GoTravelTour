using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Orden
    {
        public int OrdenId { get; set; }
        public string Estado { get; set; }
        public string NumeroOrden { get; set; }    // Codigo de la orden    
        public string NombreOrden { get; set; } 
        public string NombreClienteFinal { get; set; } //Esto es una referencia al nombre del cliente q va a consumir el servicio
        public int ClienteId { get; set; }
        public bool OFACrequired { get; set; }
        public bool HasVoucher { get; set; }
        public bool ConfirmacionOrden { get; set; }
        public string Notas { get; set; }
        public string NumeroAsistencia { get; set; }
        public string IntercomConferceNumber { get; set; }
        public Cliente Cliente { get; set; } // hay q tener en cuenta el descuento 
        public Usuario Creador { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaPeticion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int CantidadAdulto { get; set; }
        public int CantidadNino { get; set; }
        public int CantidadInfante { get; set; }
        public int IdEstimadoQB { get; set; } //id del estimado creado en cQB  para poder editarlo si esta en null es  pq a la orden no se le ha creado el estimado
        //public Sobreprecio Sobreprecio { get; set; }
        public List<OrdenVehiculo> ListaVehiculosOrden { get; set; }
        public List<OrdenTraslado> ListaTrasladoOrden { get; set; }
        public List<OrdenAlojamiento> ListaAlojamientoOrden { get; set; }
        public List<OrdenActividad> ListaActividadOrden { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioGeneralOrden { get; set; }
        public bool IsActive { get; set; }
        
    }
}
