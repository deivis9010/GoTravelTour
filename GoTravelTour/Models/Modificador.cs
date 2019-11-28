using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class Modificador
    {
        public int ModificadorId { get; set; }
        public string IdentificadorModificador { get; set; } // A|A|@I        
        public string TipoModificadorPrecio { get; set; } //Aqui hay dos opciones solamente
        public bool IsActivo { get; set; }     
        public int CantInfantes { get; set; }
        public int CantNino { get; set; }
        public int CantAdult { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public Contrato Contrato { get; set; }

         









    }
}
