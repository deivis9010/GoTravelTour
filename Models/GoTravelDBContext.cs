using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTravelTour.Models;

namespace GoTravelTour.Models
{
    public class GoTravelDBContext : DbContext
    {
        public GoTravelDBContext(DbContextOptions<GoTravelDBContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<PlanesAlimenticios> PlanesAlimenticios { get; set; }
        public DbSet<AlmacenImagenes> AlmacenImagenes { get; set; }
        public DbSet<Rutas> Rutas { get; set; }
        public DbSet<Region> Regiones { get; set; }
        public DbSet<PuntoInteres> PuntosInteres { get; set; }
        public DbSet<TipoProducto> TipoProductos { get; set; }
        public DbSet<TipoAlojamiento> TipoAlojamientos { get; set; }
        public DbSet<TipoTransporte> TipoTransportes { get; set; }
        public DbSet<TipoHabitacion> TipoHabitaciones { get; set; }
        public DbSet<NombreTemporada> NombreTemporadas { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Comodidades> Comodidades { get; set; }
        public DbSet<Distribuidor> Distribuidores { get; set; }
        public DbSet<CombinacionHuespedes> CombinacionHuespedes { get; set; }
        public DbSet<ComodidadesProductos> ComodidadesProductos { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<Modificador> Modificadores { get; set; }
        public DbSet<ModificadorProductos> ModificadorProductos { get; set; }
        public DbSet<PosibleCombinacion> PosibleCombinaciones { get; set; }
        public DbSet<PrecioAlojamiento> PrecioAlojamiento { get; set; }
        public DbSet<PrecioPlanesAlimenticios> PrecioPlanesAlimenticios { get; set; }
        public DbSet<PrecioComodidades> PrecioComodidades { get; set; }
        public DbSet<PrecioRentaAutos> PrecioRentaAutos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoDistribuidor> ProductoDistribuidores { get; set; }
        public DbSet<RangoFechas> RangoFechas { get; set; }
        public DbSet<Reglas> Reglas { get; set; }
        public DbSet<Restricciones> Restricciones { get; set; }
        public DbSet<RestriccionesPrecio> RestriccionesPrecios { get; set; }
        public DbSet<Temporada> Temporadas { get; set; }
        public DbSet<CategoriaComodidad> CategoriaComodidades { get; set; }
        public DbSet<Alojamiento> Alojamientos { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Actividad> Actividadess { get; set; }
        public DbSet<Traslado> Traslados { get; set; }
        public DbSet<PrecioTraslado> PrecioTraslados { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<CategoriaHoteles> CategoriaHoteles { get; set; }
        public DbSet<Servicio> Servicio { get; set; }
        public DbSet<TipoServicio> TipoServicio { get; set; }
        public DbSet<CategoriaAuto> CategoriaAuto { get; set; }
        public DbSet<VehiculoCategoriaAuto> VehiculoCategoriaAuto { get; set; }
        public DbSet<Sobreprecio> Sobreprecio { get; set; }
        public DbSet<PrecioServicio> PrecioServicio { get; set; }
        public DbSet<NombreHabitacion> NombreHabitacion { get; set; }
        public DbSet<ServiciosHabitacion> ServiciosHabitacion { get; set; }
        public DbSet<HabitacionServiciosHabitacion> HabitacionServiciosHabitacion { get; set; }
        public DbSet<AlojamientosPlanesAlimenticios> AlojamientosPlanesAlimenticios { get; set; }
        public DbSet<ConfiguracionVoucher> ConfiguracionVoucher { get; set; }
        public DbSet<Paquete> Paquete { get; set; }
        public DbSet<CategoriaHabitacion> CategoriaHabitacion { get; set; }
        public DbSet<HabitacionTipoHabitacion> HabitacionTipoHabitacion { get; set; }
        public DbSet<ModificadorTemporada> ModificadorTemporada { get; set; }
        public DbSet<PrecioActividad> PrecioActividad { get; set; }
        public DbSet<Orden> Orden { get; set; }
        public DbSet<GoTravelTour.Models.OrdenActividad> OrdenActividad { get; set; }
        public DbSet<GoTravelTour.Models.OrdenAlojamiento> OrdenAlojamiento { get; set; }
        public DbSet<GoTravelTour.Models.OrdenTraslado> OrdenTraslado { get; set; }
        public DbSet<GoTravelTour.Models.OrdenVehiculo> OrdenVehiculo { get; set; }
        public DbSet<GoTravelTour.Models.OrdenVehiculoPrecioRentaAuto> OrdenVehiculoPrecioRentaAuto { get; set; }
        public DbSet<GoTravelTour.Models.OrdenAlojamientoPrecioAlojamiento> OrdenAlojamientoPrecioAlojamiento { get; set; }
        public DbSet<GoTravelTour.Models.LicenciasOFAC> LicenciasOFAC { get; set; }
        public DbSet<GoTravelTour.Models.DatosPasajeroPrimario> DatosPasajeroPrimario { get; set; }
        public DbSet<GoTravelTour.Models.DatosPasajeroSecundario> DatosPasajeroSecundario { get; set; }
        public DbSet<GoTravelTour.Models.TokenQB> TokenQB { get; set; }
        public DbSet<GoTravelTour.Models.PreciosOrdenModificados> PreciosOrdenModificados { get; set; }
        public DbSet<GoTravelTour.Models.VoucherOrden> VoucherOrden { get; set; }
        
        public DbSet<GoTravelTour.Models.OrdenServicioAdicional> OrdenServicioAdicional { get; set; }
        
        public DbSet<GoTravelTour.Models.TipoServicioAdicional> TipoServicioAdicional { get; set; }
        
        public DbSet<GoTravelTour.Models.ServicioAdicional> ServicioAdicional { get; set; }
       
    



    }
}
