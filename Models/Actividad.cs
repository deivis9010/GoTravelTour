using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Actividad: Producto
    {
        public int ActividadId { get; set; }
        public string Modalidad { get; set; } //Grupal o no
        public int CantidadPersonas { get; set; }
        public int? Duracion { get; set; }
        public int? MaxDuracion { get; set; }
        public int? IdQB { get; set; }
        public bool HasTransporte { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }       
        public bool PermiteAdult { get; set; }
        public bool PermiteInfante { get; set; }
        public bool PermiteNino { get; set; }
        public Region Region { get; set; }
        [NotMapped]
        public List<Servicio> ServiciosAdicionados { get; set; } //features
    }
}
