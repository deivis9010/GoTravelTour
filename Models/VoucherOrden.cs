using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public class VoucherOrden
    {

        public int VoucherOrdenId { get; set; }
        public int OrdenId { get; set; }
        public int OrdenVehiculoId { get; set; }
        public int OrdenTrasladoId { get; set; }
        public int OrdenAlojamientoId { set; get; }
        public int OrdenActividadId { get; set; }       
        public string UrlVoucher { get; set; }
        public string Nombre { get; set; }

    }
}
