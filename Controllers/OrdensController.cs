using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using GoTravelTour.Utiles;
using Microsoft.AspNetCore.Authorization;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdensController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public OrdensController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Ordens
        [HttpGet]
        public IEnumerable<Orden> GetOrden(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<Orden> lista;
            if (col == "-1")
            {
                lista = _context.Orden
                    .Include(x=>x.Cliente)
                    .Include(x=>x.Creador)
                    
                    .OrderBy(a => a.NombreOrden)
                    .ToList();
                               

                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Orden
                    .Include(x => x.Cliente)
                    .Include(x => x.Creador)
                    .OrderBy(a => a.NombreOrden)
                    .Where(p => (p.NombreOrden.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.Orden
                    .Include(x => x.Cliente)
                    .Include(x => x.Creador)
                    .OrderBy(a => a.NombreOrden)
                    .ToPagedList(pageIndex, pageSize).ToList();

            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.NombreOrden);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.NombreOrden);

                        }
                    }

                    break;
            }
           

            return lista;
        }
        // GET: api/Ordens/Count
        [Route("Count")]
        [HttpGet]
        public int GetOrdensCount()
        {
            return _context.Orden.Count();
        }

        // GET: api/Ordens/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrden([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orden = await _context.Orden.FindAsync(id);

            if (orden == null)
            {
                return NotFound();
            }

            return Ok(orden);
        }

        // PUT: api/Ordens/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutOrden([FromRoute] int id, [FromBody] Orden orden)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orden.OrdenId)
            {
                return BadRequest();
            }

            orden.Cliente = _context.Clientes.Single(x => x.ClienteId == orden.ClienteId);
            orden.Creador = _context.Usuarios.Single(x => x.UsuarioId == orden.Creador.UsuarioId);
            orden.FechaActualizacion = DateTime.Now;

            List<OrdenVehiculo> veAborrar = _context.OrdenVehiculo.Where(x => x.OrdenId == orden.OrdenId).ToList();
            foreach(var vo in veAborrar)
            {
                _context.OrdenVehiculo.Remove(vo);
            }
           
            if (orden.ListaVehiculosOrden != null)
            {
                foreach (var vo in orden.ListaVehiculosOrden)
                {
                    vo.PrecioRentaAutos = _context.PrecioRentaAutos
                        .Include(x => x.Temporada)
                        .Include(x => x.Auto)
                        .Single(x => x.PrecioRentaAutosId == vo.PrecioRentaAutos.PrecioRentaAutosId);
                    vo.Vehiculo = _context.Vehiculos
                        .Include(x => x.Marca)
                        .Include(x => x.Modelo)
                        .Include(x => x.Proveedor)
                        .Include(x => x.PuntoInteres)
                        .Include(x => x.TipoProducto)

                        .Single(x => x.ProductoId == vo.Vehiculo.ProductoId);

                    vo.Distribuidor = _context.Distribuidores
                                             .Single(x => x.DistribuidorId == vo.DistribuidorId);
                    if (vo.LugarEntrega != null)
                        vo.LugarEntrega = _context.PuntosInteres
                                       .Include(x=>x.Region)
                                             .Single(x => x.PuntoInteresId== vo.LugarEntrega.PuntoInteresId);
                    if (vo.LugarRecogida != null)
                        vo.LugarRecogida = _context.PuntosInteres
                                             .Include(x => x.Region)
                                             .Single(x => x.PuntoInteresId == vo.LugarRecogida.PuntoInteresId);
                                            
    
                    _context.OrdenVehiculo.Add(vo);

                }
            }
            List<OrdenTraslado> trAborrar = _context.OrdenTraslado.Where(x => x.OrdenId == orden.OrdenId).ToList();
            foreach (var to in trAborrar)
            {
                _context.OrdenTraslado.Remove(to);
            }
            if (orden.ListaTrasladoOrden != null)
            {
                foreach (var to in orden.ListaTrasladoOrden)
                {
                    to.PrecioTraslado = _context.PrecioTraslados
                        .Include(x => x.Temporada)
                        .Include(x => x.Rutas)
                        .Include(x => x.Traslado)
                        .Single(x => x.PrecioTrasladoId == to.PrecioTraslado.PrecioTrasladoId);
                    to.Traslado = _context.Traslados
                        .Include(x => x.TipoTransporte)
                        .Include(x => x.Proveedor)
                        .Include(x => x.PuntoInteres)
                        .Include(x => x.TipoProducto)
                        .Single(x => x.ProductoId == to.Traslado.ProductoId);
                    to.Distribuidor = _context.Distribuidores
                                             .Single(x => x.DistribuidorId == to.DistribuidorId);
                    to.PuntoOrigen = _context.PuntosInteres
                                       .Include(x => x.Region)
                                             .Single(x => x.PuntoInteresId == to.PuntoOrigen.PuntoInteresId);
                    to.PuntoDestino = _context.PuntosInteres
                                             .Include(x => x.Region)
                                             .Single(x => x.PuntoInteresId == to.PuntoDestino.PuntoInteresId);

                    _context.OrdenTraslado.Add(to);

                }
            }

            List<OrdenActividad> acAborrar = _context.OrdenActividad.Where(x => x.OrdenId == orden.OrdenId).ToList();
            foreach (var aco in acAborrar)
            {
                _context.OrdenActividad.Remove(aco);
            }

            if (orden.ListaActividadOrden != null)
            {
                foreach (var oac in orden.ListaActividadOrden)
                {
                    oac.PrecioActividad = _context.PrecioActividad
                        .Include(x => x.Temporada)

                        .Single(x => x.PrecioActividadId == oac.PrecioActividad.PrecioActividadId);
                    oac.Actividad = _context.Actividadess
                        .Include(x => x.Proveedor)
                        .Include(x => x.PuntoInteres)
                        .Include(x => x.TipoProducto)
                        .Single(x => x.ProductoId == oac.Actividad.ProductoId);
                    oac.Distribuidor = _context.Distribuidores
                                             .Single(x => x.DistribuidorId == oac.DistribuidorId);

                    _context.OrdenActividad.Add(oac);
                }
               
            }

            List<OrdenAlojamiento> alAborrar = _context.OrdenAlojamiento.Where(x => x.OrdenId == orden.OrdenId).ToList();
            foreach (var alo in alAborrar)
            {
                _context.OrdenAlojamiento.Remove(alo);
            }

            if (orden.ListaAlojamientoOrden != null)
            {
                foreach (var oal in orden.ListaAlojamientoOrden)
                {
                    oal.PrecioAlojamiento = _context.PrecioAlojamiento
                        .Include(x => x.Temporada)
                        .Include(x => x.Contrato)
                        .Include(x => x.Habitacion)
                         .Include(x => x.TipoHabitacion)
                        .Single(x => x.PrecioAlojamientoId == oal.PrecioAlojamiento.Precio);
                    oal.Alojamiento = _context.Alojamientos
                        .Include(x => x.Proveedor)
                        .Include(x => x.PuntoInteres)
                        .Include(x => x.TipoProducto)
                        .Single(x => x.ProductoId == oal.Alojamiento.ProductoId);
                    oal.Distribuidor = _context.Distribuidores
                                             .Single(x => x.DistribuidorId == oal.DistribuidorId);
                    oal.Habitacion = _context.Habitaciones.Single(x => x.HabitacionId == oal.Habitacion.HabitacionId);

                    oal.PlanAlimenticio = _context.PlanesAlimenticios.Single(x => x.PlanesAlimenticiosId == oal.PlanesAlimenticiosId);

                    _context.OrdenAlojamiento.Add(oal);
                }
            }

            _context.Entry(orden).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Ordens
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostOrden([FromBody] Orden orden)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            orden.NumeroOrden = u.GetCodigoOrden();
           
            orden.Cliente = _context.Clientes.Single(x => x.ClienteId == orden.ClienteId);
            orden.Creador = _context.Usuarios.Single(x => x.UsuarioId == orden.Creador.UsuarioId);
            orden.FechaCreacion = DateTime.Now;

            if (orden.ListaVehiculosOrden != null)
            {
                foreach (var vo in orden.ListaVehiculosOrden)
                {
                    vo.PrecioRentaAutos = _context.PrecioRentaAutos
                        .Include(x=>x.Temporada)
                        .Include(x => x.Auto)
                        .Single(x => x.PrecioRentaAutosId == vo.PrecioRentaAutos.PrecioRentaAutosId);
                    vo.Vehiculo = _context.Vehiculos
                        .Include(x => x.Marca)
                        .Include(x => x.Modelo)
                        .Include(x => x.Proveedor)
                        .Include(x => x.PuntoInteres)
                        .Include(x => x.TipoProducto)                        

                        .Single(x => x.ProductoId == vo.Vehiculo.ProductoId);
                    vo.Distribuidor = _context.Distribuidores
                                             .Single(x => x.DistribuidorId == vo.DistribuidorId);
                    if(vo.LugarEntrega!=null)
                    vo.LugarEntrega = _context.PuntosInteres
                                       .Include(x=>x.Region)
                                             .Single(x => x.PuntoInteresId== vo.LugarEntrega.PuntoInteresId);
                    if (vo.LugarRecogida != null)
                        vo.LugarRecogida = _context.PuntosInteres
                                             .Include(x => x.Region)
                                             .Single(x => x.PuntoInteresId == vo.LugarRecogida.PuntoInteresId);

                }
            }

            if (orden.ListaTrasladoOrden != null)
            {
                foreach (var to in orden.ListaTrasladoOrden)
                {
                    to.PrecioTraslado = _context.PrecioTraslados
                        .Include(x => x.Temporada)
                        .Include(x => x.Rutas)
                        .Include(x => x.Traslado)
                        .Single(x => x.PrecioTrasladoId == to.PrecioTraslado.PrecioTrasladoId);
                    to.Traslado = _context.Traslados
                        .Include(x => x.TipoTransporte)                        
                        .Include(x => x.Proveedor)
                        .Include(x => x.PuntoInteres)
                        .Include(x => x.TipoProducto)
                        .Single(x => x.ProductoId == to.Traslado.ProductoId);
                    to.Distribuidor = _context.Distribuidores
                                             .Single(x => x.DistribuidorId == to.DistribuidorId);
                    to.PuntoOrigen = _context.PuntosInteres
                                       .Include(x => x.Region)
                                             .Single(x => x.PuntoInteresId == to.PuntoOrigen.PuntoInteresId);
                    to.PuntoDestino = _context.PuntosInteres
                                             .Include(x => x.Region)
                                             .Single(x => x.PuntoInteresId == to.PuntoDestino.PuntoInteresId);



                }
            }

            if (orden.ListaActividadOrden != null)
            {
                foreach (var oac in orden.ListaActividadOrden)
                {
                    oac.PrecioActividad = _context.PrecioActividad
                        .Include(x => x.Temporada)
                        
                        .Single(x => x.PrecioActividadId == oac.PrecioActividad.PrecioActividadId);
                    oac.Actividad = _context.Actividadess                        
                        .Include(x => x.Proveedor)
                        .Include(x => x.PuntoInteres)
                        .Include(x => x.TipoProducto)
                        .Single(x => x.ProductoId == oac.Actividad.ProductoId);
                    oac.Distribuidor = _context.Distribuidores
                                             .Single(x => x.DistribuidorId == oac.DistribuidorId);
                    

                }
            }

            if (orden.ListaAlojamientoOrden != null)
            {
                foreach (var oal in orden.ListaAlojamientoOrden)
                {
                    oal.PrecioAlojamiento = _context.PrecioAlojamiento
                        .Include(x => x.Temporada)
                        .Include(x=>x.Contrato)
                        .Include(x => x.Habitacion)
                         .Include(x => x.TipoHabitacion)
                        .Single(x => x.PrecioAlojamientoId == oal.PrecioAlojamiento.Precio);
                    oal.Alojamiento = _context.Alojamientos
                        .Include(x => x.Proveedor)
                        .Include(x => x.PuntoInteres)
                        .Include(x => x.TipoProducto)
                        .Single(x => x.ProductoId == oal.Alojamiento.ProductoId);
                    oal.Distribuidor = _context.Distribuidores
                                             .Single(x => x.DistribuidorId == oal.DistribuidorId);
                    oal.Habitacion = _context.Habitaciones.Single(x => x.HabitacionId == oal.Habitacion.HabitacionId);

                    oal.PlanAlimenticio = _context.PlanesAlimenticios.Single(x => x.PlanesAlimenticiosId == oal.PlanesAlimenticiosId);
                  

                }
            }

            _context.Orden.Add(orden);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }

        // DELETE: api/Ordens/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrden([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orden = await _context.Orden.FindAsync(id);
            if (orden == null)
            {
                return NotFound();
            }

            _context.Orden.Remove(orden);
            await _context.SaveChangesAsync();

            return Ok(orden);
        }

        private bool OrdenExists(int id)
        {
            return _context.Orden.Any(e => e.OrdenId == id);
        }


        // POST: api/Ordens
        [HttpPost]
        [Route("addVehiculo")]
        public async Task<IActionResult> PostAddVehiculo( [FromBody] OrdenVehiculo ordenVehiculo, int idOrden = 0)
        {
            if (!ModelState.IsValid || idOrden <= 0)
            {
                return BadRequest(ModelState);
            }

            Orden orden = _context.Orden.Single(x=>x.OrdenId == idOrden);
            if(orden.ListaVehiculosOrden == null)
            {
                orden.ListaVehiculosOrden = new List<OrdenVehiculo>();
            }
            orden.ListaVehiculosOrden.Add(ordenVehiculo);
            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }

        // POST: api/Ordens
        [HttpPost]
        [Route("deleteVehiculo")]
        public async Task<IActionResult> PostDeleteVehiculo([FromBody] OrdenVehiculo ordenVehiculo, int idOrden = 0)
        {
            if (!ModelState.IsValid || idOrden <= 0)
            {
                return BadRequest(ModelState);
            }

            Orden orden = _context.Orden.Single(x => x.OrdenId == idOrden);
            
            orden.ListaVehiculosOrden.Remove(ordenVehiculo);
            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }

        // POST: api/Ordens
        [HttpPost]
        [Route("addAlojamiento")]
        public async Task<IActionResult> PostAddAlojamiento([FromBody] OrdenAlojamiento ordenAlojamiento, int idOrden = 0)
        {
            if (!ModelState.IsValid || idOrden <= 0)
            {
                return BadRequest(ModelState);
            }

            Orden orden = _context.Orden.Single(x => x.OrdenId == idOrden);
            if (orden.ListaAlojamientoOrden == null)
            {
                orden.ListaAlojamientoOrden = new List<OrdenAlojamiento>();
            }
            orden.ListaAlojamientoOrden.Add(ordenAlojamiento);
            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }

        // POST: api/Ordens
        [HttpPost]
        [Route("deleteAlojamiento")]
        public async Task<IActionResult> PostDeleteAlojamiento([FromBody] OrdenAlojamiento ordenAlojamiento, int idOrden = 0)
        {
            if (!ModelState.IsValid || idOrden <= 0)
            {
                return BadRequest(ModelState);
            }

            Orden orden = _context.Orden.Single(x => x.OrdenId == idOrden);

            orden.ListaAlojamientoOrden.Remove(ordenAlojamiento);
            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }


        // POST: api/Ordens
        [HttpPost]
        [Route("addActividad")]
        public async Task<IActionResult> PostAddActividad([FromBody] OrdenActividad ordenActividad, int idOrden = 0)
        {
            if (!ModelState.IsValid || idOrden <= 0)
            {
                return BadRequest(ModelState);
            }

            Orden orden = _context.Orden.Single(x => x.OrdenId == idOrden);
            if (orden.ListaActividadOrden == null)
            {
                orden.ListaActividadOrden = new List<OrdenActividad>();
            }
            orden.ListaActividadOrden.Add(ordenActividad);
            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }

        // POST: api/Ordens
        [HttpPost]
        [Route("deleteActividad")]
        public async Task<IActionResult> PostDeleteActividad([FromBody] OrdenActividad ordenActividad, int idOrden = 0)
        {
            if (!ModelState.IsValid || idOrden <= 0)
            {
                return BadRequest(ModelState);
            }

            Orden orden = _context.Orden.Single(x => x.OrdenId == idOrden);

            orden.ListaActividadOrden.Remove(ordenActividad);
            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }

        // POST: api/Ordens
        [HttpPost]
        [Route("addTraslado")]
        public async Task<IActionResult> PostAddTraslado([FromBody] OrdenTraslado ordenTraslado, int idOrden = 0)
        {
            if (!ModelState.IsValid || idOrden <= 0)
            {
                return BadRequest(ModelState);
            }

            Orden orden = _context.Orden.Single(x => x.OrdenId == idOrden);
            if (orden.ListaTrasladoOrden == null)
            {
                orden.ListaTrasladoOrden = new List<OrdenTraslado>();
            }
            orden.ListaTrasladoOrden.Add(ordenTraslado);
            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }

        // POST: api/Ordens
        [HttpPost]
        [Route("deleteTraslado")]
        public async Task<IActionResult> PostDeleteTraslado([FromBody] OrdenTraslado ordenTraslado, int idOrden = 0)
        {
            if (!ModelState.IsValid || idOrden <= 0)
            {
                return BadRequest(ModelState);
            }

            Orden orden = _context.Orden.Single(x => x.OrdenId == idOrden);

            orden.ListaTrasladoOrden.Remove(ordenTraslado);
            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.OrdenId }, orden);
        }


        // GET: api/Ordens/Count
        [Route("EstadoCount")]
        [HttpGet]
        public int GetEstadoOrdensCount(string estado)
        {
            return _context.Orden.Where(x=>x.Estado== estado).Count();
        }

    }
}