using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class DatosPasajeroSecundario
    {
        public int DatosPasajeroSecundarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string CiudadSalida { get; set; }
        public string Nacionalidad { get; set; }
        public string NumeroPasaporte { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Calle { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }
        public bool AffiDavitRequired { get; set; }
        public bool AffiDavitOk { get; set; }
        public string NombreImagen { get; set; }      
        public string ImageContent { get; set; }
        public LicenciasOFAC LicenciasOFAC { get; set; }
        public DatosPasajeroPrimario DatosPasajeroPrimario { get; set; }
    }
}
