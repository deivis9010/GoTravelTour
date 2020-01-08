using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Restricciones
    {
        public int RestriccionesId {get; set;}
        public int Minimo { get; set; }
        public int Maximo { get; set; }
        public bool IsActivo { get; set; }
        //public int TemporadaId { get; set; }             
        public Temporada Temporada { get; set; }

    }
}
