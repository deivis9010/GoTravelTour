using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Alojamiento: Producto
    {
        public int AlojamientoId { get; set; }
        public string Categoria { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public bool PermiteAdult { get; set; }
        public bool PermiteInfante { get; set; }
        public bool PermiteNino { get; set; }
        public int TipoAlojamientoId { get; set; }
        public TipoAlojamiento TipoAlojamiento { get; set; }


    }
}
