using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateTime? FechaI { get; set; }
        public DateTime? FechaF { get; set; }        
        public bool PermiteHacerCopia { get; set; }
        public Proveedor Proveedor  { get; set; } //para usar como filtros para los productos distribuidos       
        public Contrato Contrato { get; set; }
        public List<ModificadorProductos> ListaHoteles { get; set; } //Hoteles sobre los cuales es valido el modificador
        public List<Reglas> ListaReglas { get; set; } // una x la cantidad de personas en el modificador



    }
}
