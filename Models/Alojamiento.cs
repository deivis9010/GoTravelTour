using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Alojamiento: Producto
    {
        public int AlojamientoId { get; set; }
        public int CategoriaHotelesId { get; set; }
        public int NumeroEstrellas { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string Direccion { get; set; }
        public string PoliticaNino { get; set; }
        public string PoliticaCancelacion { get; set; }
        public string InfoLegal { get; set; }
        public int? EdadAdultoMax { get; set; }
        public int? EdadNinoMax { get; set; }
        public int? EdadInfanteMax { get; set; }
        public int? EdadAdultoMin { get; set; }
        public int? EdadNinoMin { get; set; }
        public int? EdadInfanteMin { get; set; }
        public bool? PermiteMascota { get; set; }
        public bool? PermiteAdult { get; set; }
        public bool? PermiteInfante { get; set; }
        public bool? PermiteNino { get; set; }
        public int TipoAlojamientoId { get; set; }
        public TipoAlojamiento TipoAlojamiento { get; set; }
        public List<ModificadorProductos> ListaHoteles { get; set; }
        public List<AlojamientosPlanesAlimenticios> ListaPlanesAlimenticios { get; set; }
        public CategoriaHoteles CategoriaHoteles { get; set; }



    }
}
