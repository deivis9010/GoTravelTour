using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Rutas
    {
        public int RutasId { get; set; }       
        
        public PuntoInteres PuntoInteresOrigen { get; set; }         
        public PuntoInteres PuntoInteresDestino { get; set; }
        public int Distancia { get; set; }

    }
}
