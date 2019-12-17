using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Models
{
    public abstract class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string SKU { get; set; }
        public string DescripcionCorta { get; set; }
        public bool IsActivo { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public bool PermiteAdult { get; set; }
        public bool PermiteInfante { get; set; }
        public bool PermiteNino { get; set; }
        public bool PermiteHacerCopia { get; set; }
        public int ProveedorId { get; set; }
        //public int RegionId { get; set; }
        //public int PuntoInteresId { get; set; }
        public int TipoProductoId { get; set; }
        public string Notas { get; set; }        
        public Region Region { get; set; }
        public PuntoInteres PuntoInteres { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public Proveedor Proveedor { get; set; }
        public List<ProductoDistribuidor> ListaDistribuidoresProducto { get; set; }
        public List<ComodidadesProductos>ListaComodidades { get; set; }
    }
}
