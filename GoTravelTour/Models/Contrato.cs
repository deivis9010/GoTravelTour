using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Contrato
    {
        public int ContratoId { get; set; }
        public string Nombre { get; set; }
        public bool IsActivo { get; set; }
        public bool OfertaEspecial { get; set; }
        public string CodigoOfertaEspecial { get; set; }       
        public DateTime? FechaInicioTravel { get; set; }
        public DateTime? FechaFinTravel { get; set; }
        public DateTime? FechaInicioBooking { get; set; }
        public DateTime? FechaFinBooking { get; set; }
        public int TipoProductoId { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public List<Temporada> Temporadas { get; set; }
                     
        //ListaProductos(los productos que estan en el contrato) Esto Genera una tabla nueva





    }
}
