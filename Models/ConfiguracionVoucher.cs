using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class ConfiguracionVoucher
    {
        public int ConfiguracionVoucherId { get; set; }
        public int TipoProductoId { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public string Condiciones { get; set; }
        public string InfoAgente { get; set; }
        public string TelefonoAsistencia { get; set; }
        public string Correo { get; set; }
        public string ImageContent { get; set; }
        public string TipoImagen { get; set; }
        public bool IsActivo { get; set; }

    }
}
