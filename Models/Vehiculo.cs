using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Vehiculo: Producto
    {
        public int VehiculoId { get; set; }
        public string TipoTransmision { get; set; }
        public string TieneSeguro { get; set; }
        public int CantidadPlazas { get; set; }
        public int MarcaId { get; set; }
        public int ModeloId { get; set; }        
        public Marca Marca { get; set; }
        public Modelo Modelo { get; set; }    
        [NotMapped]
        public List<VehiculoCategoriaAuto> ListaCategorias { get; set; }


    }
}
