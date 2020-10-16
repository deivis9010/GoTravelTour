using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using Microsoft.AspNetCore.Authorization;
using PagedList;
using GoTravelTour.Utiles;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public VehiculoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Vehiculoes
        [HttpGet]
        public IEnumerable<Vehiculo> GetVehiculos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<Vehiculo> lista;
            if (col == "-1")
            {
                lista = _context.Vehiculos
                    //.Include(v => v.Marca)
                    .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                    .Include(a => a.PuntoInteres)
                   //  .Include(v => v.TipoProducto)
                   //  .Include(v => v.ListaDistribuidoresProducto)
                   .Where(x => x.ProveedorId == idProveedor)
                    .OrderBy(a => a.Nombre)
                    .ToList();

                foreach (Vehiculo v in lista)
                {
                    v.ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == v.ProductoId).ToList();
                }
                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Vehiculos
                    //.Include(v => v.Marca)
                    .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                    .Include(a => a.PuntoInteres)
                    //  .Include(v => v.TipoProducto)
                    //.Include(v => v.ListaDistribuidoresProducto)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
                foreach (Vehiculo v in lista)
                {
                    v.ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == v.ProductoId).ToList();
                }
            }
            else
            {
                lista = _context.Vehiculos
                   // .Include(v => v.Marca)
                   .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                    .Include(a => a.PuntoInteres)
                    // .Include(v => v.TipoProducto)
                    //   .Include(v => v.ListaDistribuidoresProducto)
                    .ToPagedList(pageIndex, pageSize).ToList();
                foreach (Vehiculo v in lista)
                {
                    v.ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == v.ProductoId).ToList();
                }
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Nombre);

                        }




                    }

                    break;
            }

            return lista;
        }
        // GET: api/Vehiculoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetVehiculoCount()
        {
            return _context.Vehiculos.Count();
        }

        // GET: api/Vehiculoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehiculo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*  var vehiculo = await _context.Vehiculos
                  .FindAsync(id);*/
            var vehiculo = _context.Vehiculos
               .Include(v => v.Marca)
               .Include(v => v.Modelo)
               .Include(v => v.Proveedor)
               .Include(v => v.TipoProducto)
               .Include(a => a.PuntoInteres)
               .Include(v => v.ListaDistribuidoresProducto)
           .Single(x => x.ProductoId == id);

            vehiculo.ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == vehiculo.ProductoId).ToList();

            if (vehiculo == null)
            {
                return NotFound();
            }

            return Ok(vehiculo);
        }

        // PUT: api/Vehiculoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutVehiculo([FromRoute] int id, [FromBody] Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vehiculo.ProductoId)
            {
                return BadRequest();
            }

            if (_context.Vehiculos.Any(c => c.Nombre == vehiculo.Nombre && c.ProductoId != id && c.ProveedorId == vehiculo.ProveedorId))
            {
                return CreatedAtAction("GetVehiculos", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            List<VehiculoCategoriaAuto> ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == id).ToList();
            int i = 0;
            while (i < ListaCategorias.Count())
            {
                VehiculoCategoriaAuto c = ListaCategorias[i];
                bool esta = false;
                foreach (VehiculoCategoriaAuto c2 in vehiculo.ListaCategorias)
                {
                    if (c.VehiculoCategoriaAutoId == c2.VehiculoCategoriaAutoId)
                    {
                        esta = true;
                        break;
                    }

                }
                if (!esta && c.VehiculoCategoriaAutoId != 0)
                {
                    _context.VehiculoCategoriaAuto.Remove(c);
                    ListaCategorias.Remove(c);
                    i--;
                }

                i++;


            }
            await _context.SaveChangesAsync();
            i = 0;
            while (i < vehiculo.ListaCategorias.Count)
            {
                VehiculoCategoriaAuto c = vehiculo.ListaCategorias[i];
                if (c.VehiculoCategoriaAutoId == 0)
                {
                    c.ProductoId = vehiculo.ProductoId;
                    _context.VehiculoCategoriaAuto.Add(c);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    /* _context.Entry(c).State = EntityState.Modified;
                     await _context.SaveChangesAsync();*/

                }
                i++;
            }
            List<ComodidadesProductos> comodidades = _context.ComodidadesProductos.Where(x => x.ProductoId == vehiculo.ProductoId).ToList();
            foreach (var item in comodidades)
            {
                _context.ComodidadesProductos.Remove(item);
            }

            List<ProductoDistribuidor> distribuidors = _context.ProductoDistribuidores.Where(x => x.ProductoId == vehiculo.ProductoId).ToList();
            foreach (var item in distribuidors)
            {
                _context.ProductoDistribuidores.Remove(item);
            }
            if (vehiculo.ListaDistribuidoresProducto != null)
                foreach (var item in vehiculo.ListaDistribuidoresProducto)
                {
                    item.ProductoId = vehiculo.ProductoId;
                    _context.ProductoDistribuidores.Add(item);
                }
            if (vehiculo.ListaComodidades != null)
                foreach (var item in vehiculo.ListaComodidades)
                {
                    item.ProductoId = vehiculo.ProductoId;
                    _context.ComodidadesProductos.Add(item);
                }

            vehiculo.Proveedor = _context.Proveedores.First(x => x.ProveedorId == vehiculo.ProveedorId);
            if (vehiculo.PuntoInteres != null)
                vehiculo.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == vehiculo.PuntoInteres.PuntoInteresId);
            vehiculo.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == vehiculo.TipoProductoId);
            vehiculo.Marca = _context.Marcas.First(x => x.MarcaId == vehiculo.MarcaId);
            vehiculo.Modelo = _context.Modelos.First(x => x.ModeloId == vehiculo.ModeloId);



            _context.Entry(vehiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVehiculo", new { id = vehiculo.ProductoId }, vehiculo);
        }

        // POST: api/Vehiculoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostVehiculo([FromBody] Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Vehiculos.Any(c => c.Nombre == vehiculo.Nombre && c.ProveedorId == vehiculo.ProveedorId))
            {
                return CreatedAtAction("GetVehiculos", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            vehiculo.SKU = u.GetSKUCodigo();
            vehiculo.Proveedor = _context.Proveedores.First(x => x.ProveedorId == vehiculo.ProveedorId);
            if (vehiculo.PuntoInteres != null)
                vehiculo.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == vehiculo.PuntoInteres.PuntoInteresId);
            vehiculo.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == vehiculo.TipoProductoId);
            vehiculo.Marca = _context.Marcas.First(x => x.MarcaId == vehiculo.MarcaId);
            vehiculo.Modelo = _context.Modelos.First(x => x.ModeloId == vehiculo.ModeloId);


            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            if (vehiculo.ListaCategorias.Count() > 0)
            {
                foreach (VehiculoCategoriaAuto vca in vehiculo.ListaCategorias)
                {
                    VehiculoCategoriaAuto temp = new VehiculoCategoriaAuto();
                    temp.CategoriaAutoId = vca.CategoriaAutoId;
                    temp.ProductoId = vehiculo.ProductoId;
                    _context.VehiculoCategoriaAuto.Add(temp);
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetVehiculo", new { id = vehiculo.ProductoId }, vehiculo);
        }

        // DELETE: api/Vehiculoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehiculo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return Ok(vehiculo);
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.ProductoId == id);
        }


        // GET: api/Vehiculoes/FiltrosCount
        [HttpGet]
        [Route("FiltrosCount")]
        public IEnumerable<Contrato> GetContratosByFiltrosCount(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos

                .Where(a => a.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);

                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor != -1)
            {

                lista = _context.Contratos

               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
               .OrderBy(a => a.Nombre)
               .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {


                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);

                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor == -1)
            {
                lista = _context.Contratos

                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {
                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                    }
                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor != -1)
            {
                lista = _context.Contratos

                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {
                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                    }
                return lista;
            }


            return lista;

        }


        // GET: api/Vehiculoes/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Contrato> GetContratosByFiltros(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0, int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }

                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }


                        }


                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor != -1)
            {

                lista = _context.Contratos
               .Include(a => a.Distribuidor)
               .Include(a => a.Temporadas)
               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
               .OrderBy(a => a.Nombre)
               .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {


                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor == -1)
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {


                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor != -1)
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {


                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;

                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                  .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }


            return lista;

        }

        private void FiltrarProductoProveedor(int idProveedor, int idProducto, Contrato contrato, int pageIndex = 1, int pageSize = 1)
        {
            if (idProducto == 0 && idProveedor == 0)
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE && x.Producto.ProveedorId == idProveedor)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
        }

        private int CantidadProductoProveedor(int idProveedor, int idProducto, Contrato contrato)
        {

            if (idProducto == 0 && idProveedor == 0)
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE).Count();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                return _context.ProductoDistribuidores
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE && x.ProductoId == idProducto).Count();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                return _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE && x.Producto.ProveedorId == idProveedor).Count();
            }
            else
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto).Count();
            }
        }


        // GET: api/Vehiculoes/BuscarOrdenCount
        [HttpPost]
        [Route("BuscarOrdenCount")]
        public int GetOrdenVehiculosCount([FromBody] BuscadorVehiculo buscador, int pageIndex = 1, int pageSize = 1)
        {
            return 50;
        }

        
        // GET: api/Vehiculoes/BuscarOrden
        [HttpPost]
        [Route("BuscarOrden")]
        public List<OrdenVehiculo> GetOrdenVehiculos([FromBody] BuscadorVehiculo buscador, int pageIndex = 1, int pageSize = 1000)
        {
            // TODO agregar el calculo de la orden teniendo en  cuenta

            /// surcharger

            List<OrdenVehiculo> lista = new List<OrdenVehiculo>(); //Lista  a devolver (candidatos)

            //Para saber que autos entran en la categoria pasada por parametros
            // List<VehiculoCategoriaAuto> cats = _context.VehiculoCategoriaAuto.Where(x => x.CategoriaAuto.CategoriaAutoId == buscador.CategoriaAuto.CategoriaAutoId).ToList();

            //Se buscan todos los auto con la transmision pasada por parametros
            List<Vehiculo> vehiculos;
            if (buscador.Marca != null && buscador.Marca.MarcaId > 0)
               vehiculos = _context.Vehiculos.Include(x=>x.ListaDistribuidoresProducto).Where(x => x.IsActivo && x.TipoTransmision == buscador.TipoTransmision && x.MarcaId == buscador.Marca.MarcaId).ToList();
            else
                 vehiculos = _context.Vehiculos.Include(x => x.ListaDistribuidoresProducto).Where(x => x.IsActivo && x.TipoTransmision == buscador.TipoTransmision ).ToList();


            foreach (var v in vehiculos) //Se recorren los vehiculos que coinciden con el tipo de transmision
            {

                foreach (var dist in v.ListaDistribuidoresProducto)
                {

                    OrdenVehiculo ov = new OrdenVehiculo();
                    int cantDiasGenenarl = (buscador.FechaEntrega - buscador.FechaRecogida).Days; //Cant. de dias a reservar
                    int cantDias = 0; // auxilar para rangos
                    int DiasRestantes = cantDiasGenenarl; // para saber que cantidad de dias son extra a las restricciones
                    ov.FechaRecogida = buscador.FechaRecogida;
                    ov.FechaEntrega = buscador.FechaEntrega;
                    Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                   
                    ov.Vehiculo = v;
                    bool agregarOrden = true;

                    List<PrecioRentaAutos> precios = _context.PrecioRentaAutos.Include(x => x.Temporada.ListaFechasTemporada)
                    .Include(x => x.Temporada.Contrato.Distribuidor)
                    .Where(x => x.ProductoId == v.ProductoId && x.Temporada.Contrato.Distribuidor.DistribuidorId == dist.DistribuidorId).ToList();
                    PrecioRentaAutos ultimoPrecio = new PrecioRentaAutos();
                    if(!precios.Any())
                        continue;
                    ov.ListaPreciosRentaAutos = new List<OrdenVehiculoPrecioRentaAuto>();
                    foreach (var p in precios)
                    {
                      
                        
                        if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.FechaRecogida && buscador.FechaRecogida <= x.FechaFin) ||
                         (x.FechaFin >= buscador.FechaEntrega && buscador.FechaEntrega >= x.FechaInicio))) // si la fecha buscada esta en el rango de precios
                        {
                            ultimoPrecio = p;

                            //Se obtienen las restricciones ordenadas por el valor maximo de dias para calcular precio segun cantidad de dias
                            List<Restricciones> restricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == p.Temporada.TemporadaId).OrderBy(x => x.Minimo).ToList();
                            OrdenVehiculoPrecioRentaAuto ovpra = new OrdenVehiculoPrecioRentaAuto();
                            
                            ovpra.PrecioRentaAutos = p;
                            ov.ListaPreciosRentaAutos.Add(ovpra);
                            ov.Distribuidor = p.Temporada.Contrato.Distribuidor;
                            try
                            {

                                switch (p.Temporada.Contrato.FormaCobro)// 2 - por dia 1 - PrimeraTemp 3 - UltimaTemp
                                {
                                    case 2:
                                        {
                                            Met_CalcularPrecioAutoPorDia(buscador, v, p, ov, cantDiasGenenarl, ref cantDias, ref DiasRestantes, restricciones);
                                            break;
                                        }
                                    case 1:
                                        {
                                            Met_CalcularPrecioAutoPorPrimeraTemporada(buscador, v, p, ov, cantDiasGenenarl, ref cantDias, ref DiasRestantes, restricciones);

                                            break;
                                        }
                                    case 3:
                                        {
                                            Met_CalcularPrecioAutoPorSegundaTemporada(buscador, v, p, ov, cantDiasGenenarl, ref cantDias, ref DiasRestantes, restricciones);
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }

                            }
                            catch
                            {
                                agregarOrden = false;
                                break;
                            }

                           

                        }
                        if (DiasRestantes == 0)
                            break;
                    }
                    if (!agregarOrden)
                        continue;
                    ov.PrecioOrden += (DiasRestantes * ultimoPrecio.DiasExtra); //+ (cantDiasGenenarl * ultimoPrecio.Seguro);
                    ov.PrecioOrden += (DiasRestantes * ultimoPrecio.Seguro); 



                    List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE).ToList();

                    foreach (Sobreprecio s in sobreprecios)
                    {
                        if (s.PrecioDesde <= ov.PrecioOrden && ov.PrecioOrden <= s.PrecioHasta)
                        {
                            ov.Sobreprecio = s;
                            decimal valorAplicado = 0;
                            if (s.PagoPorDia)
                            {
                                if (s.ValorDinero != null)
                                {
                                    valorAplicado = cantDiasGenenarl * (decimal)s.ValorDinero;
                                    ov.PrecioOrden += valorAplicado + (valorAplicado * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplicado = cantDiasGenenarl * ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    ov.PrecioOrden += valorAplicado + (valorAplicado * c.Descuento / 100);
                                }

                            }
                            else
                            {

                                if (s.ValorDinero != null)
                                {
                                    valorAplicado = (decimal)s.ValorDinero;
                                    ov.PrecioOrden += valorAplicado + ((decimal)s.ValorDinero * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplicado = ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    ov.PrecioOrden += valorAplicado + (ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100) * c.Descuento / 100);
                                }

                            }
                            ov.ValorSobreprecioAplicado = valorAplicado;
                            break;
                        }

                    }

                    if (agregarOrden && ov.PrecioOrden > 0)
                        lista.Add(ov);


                }



            }


            return lista.OrderByDescending(x => x.PrecioOrden).ToPagedList(pageIndex,10).ToList();

        }


       


        private void Met_CalcularPrecioAutoPorSegundaTemporada(BuscadorVehiculo buscador, Vehiculo v, PrecioRentaAutos p, OrdenVehiculo ov, int cantDiasGenenarl, ref int cantDias, ref int DiasRestantes, List<Restricciones> restricciones)
        {
            int i = 0;
            bool encontroRangoValido = false; //es para saber si la cantidad de dias a rentar entra en el rango de alguna restriccion

            List<RangoFechas> listaRangos = p.Temporada.ListaFechasTemporada.OrderBy(x => x.FechaInicio).ToList();
            //Recorro todas los rangos de fecha para ir calculando precio total
            while (i < listaRangos.Count)
            {
                RangoFechas rf = listaRangos[i];
                Restricciones rt = new Restricciones();// Se usara para obtener el rango mayor de dias a alquilar y poder calcular precios a partir de ahi
                if (rf.FechaInicio <= buscador.FechaEntrega && buscador.FechaEntrega <= rf.FechaFin)
                {
                    //Si fecha de recogida cae en rango la cantidad de dias sera la general 
                    cantDias = cantDiasGenenarl;
                    foreach (var item in restricciones)// se evalua por restricciones el valor de la cantidad de dias
                    {
                        
                        if (item.Minimo <= cantDias && cantDias <= item.Maximo)// si coincide la cantidad de dias con el rango de una restriccion se calcula
                        {
                            rt = item;
                            ov.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio * cantDias;
                            ov.PrecioOrden += cantDias * p.Seguro;
                            DiasRestantes -= cantDias; // se descuentan los dias que han sido incluidos en el precio
                            encontroRangoValido = true;
                            break;
                        }
                    }
                    if (!encontroRangoValido && restricciones.Any())
                    {
                        var rtMax = restricciones.Last();
                        var rtMin = restricciones.First();
                        if (cantDias < rtMin.Minimo)
                        {
                            //cantDias = rtMin.Minimo;
                            DiasRestantes -= cantDias;
                            rt = rtMin;
                        }
                        else if (cantDias > rtMax.Maximo)
                        {
                            cantDias = rtMax.Maximo;
                            DiasRestantes -= cantDias;
                            rt = rtMax;
                        }

                        ov.PrecioOrden += cantDias * _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == rt.RestriccionesId).Precio;
                        ov.PrecioOrden += cantDias * p.Seguro;
                    }
                }
                i++;
            }
        }

        private void Met_CalcularPrecioAutoPorPrimeraTemporada(BuscadorVehiculo buscador, Vehiculo v, PrecioRentaAutos p, OrdenVehiculo ov, int cantDiasGenenarl, ref int cantDias, ref int DiasRestantes, List<Restricciones> restricciones)
        {
            int i = 0;
            bool encontroRangoValido = false; //es para saber si la cantidad de dias a rentar entra en el rango de alguna restriccion
            List<RangoFechas> listaRangos = p.Temporada.ListaFechasTemporada.OrderBy(x => x.FechaInicio).ToList();
            //Recorro todas los rangos de fecha para ir calculando precio total
            while (i < listaRangos.Count)
            {
                RangoFechas rf = listaRangos[i];
                Restricciones rt = new Restricciones();// Se usara para obtener el rango mayor de dias a alquilar y poder calcular precios a partir de ahi
                if (rf.FechaInicio <= buscador.FechaRecogida && buscador.FechaRecogida <= rf.FechaFin)
                {
                    //Si fecha de recogida cae en rango la cantidad de dias sera la general 
                    cantDias = cantDiasGenenarl;
                    foreach (var item in restricciones)// se evalua por restricciones el valor de la cantidad de dias
                    {
                        
                        if (item.Minimo <= cantDias && cantDias <= item.Maximo)// si coincide la cantidad de dias con el rango de una restriccion se calcula
                        {
                            rt = item;
                            ov.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio * cantDias;
                            ov.PrecioOrden += cantDias * p.Seguro;
                            DiasRestantes -= cantDias; // se descuentan los dias que han sido incluidos en el precio
                            encontroRangoValido = true;

                            break;
                        }
                    }
                    if (!encontroRangoValido && restricciones.Any())
                    {
                        var rtMax = restricciones.Last();
                        var rtMin = restricciones.First();
                        if (cantDias < rtMin.Minimo)
                        {
                            //cantDias = rtMin.Minimo;
                            DiasRestantes -= cantDias;
                            rt = rtMin;
                        }
                        else if (cantDias > rtMax.Maximo)
                        {
                            cantDias = rtMax.Maximo;
                            DiasRestantes -= cantDias;
                            rt = rtMax;
                        }

                        ov.PrecioOrden += cantDias * _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == rt.RestriccionesId).Precio;
                        ov.PrecioOrden += cantDias * p.Seguro;
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Este metodo realiza el procedimiento de ir acumulando el valor de precio del auto
        /// segun las fechas y los dias de renta
        /// </summary>
        /// <param name="buscador"> parametros de busqueda</param>
        /// <param name="v">Auto que quiere rentar</param>
        /// <param name="p">atributos precio del auto a rentar</param>
        /// <param name="ov">Orden del auto a rentar</param>
        /// <param name="cantDiasGenenarl">Cantidad de dias a resevar</param>
        /// <param name="cantDias">variable auxiliar para llevar la cantidad de dias en un rango de fecha</param>
        /// <param name="DiasRestantes">dias que van a ir quedando luego de se vayan calculadno los dias segun los rangos de fechas en las temporadsa</param>
        /// <param name="restricciones">Restricciones de Precio para el vehiculos</param>
        private void Met_CalcularPrecioAutoPorDia(BuscadorVehiculo buscador, Vehiculo v, PrecioRentaAutos p, OrdenVehiculo ov, int cantDiasGenenarl, ref int cantDias, ref int DiasRestantes, List<Restricciones> restricciones)
        {
            int i = 0;
            bool encontroRangoValido = false; //es para saber si la cantidad de dias a rentar entra en el rango de alguna restriccion
            List<RangoFechas> listaRangos = p.Temporada.ListaFechasTemporada.OrderBy(x => x.FechaInicio).ToList();
            //Recorro todas los rangos de fecha para ir calculando precio total
            while (i < listaRangos.Count && DiasRestantes > 0)
            {
                RangoFechas rf = listaRangos[i];
                Restricciones rt = new Restricciones();// Se usara para obtener el rango mayor de dias a alquilar y poder calcular precios a partir de ahi

                if (rf.FechaInicio <= buscador.FechaRecogida && buscador.FechaRecogida <= rf.FechaFin &&
                  rf.FechaFin >= buscador.FechaEntrega && buscador.FechaEntrega >= rf.FechaInicio)
                {
                    //Si el el rago de la reserva cae completamente en un rango con la cantidad de dias general se calcula el precio
                    cantDias = cantDiasGenenarl;

                    foreach (var item in restricciones) // se evalua por restricciones el valor de la cantidad de dias
                    {
                        
                        if (item.Minimo <= cantDias && cantDias <= item.Maximo) // si coincide la cantidad de dias con el rango de una restriccion se calcula
                        {
                            rt = item;
                            ov.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio * cantDias;
                            ov.PrecioOrden += cantDias * p.Seguro;
                            DiasRestantes -= cantDias; // se descuentan los dias que han sido incluidos en el precio
                            encontroRangoValido = true;
                            break;
                        }
                    }
                    if (!encontroRangoValido && restricciones.Any())
                    {
                        var rtMax = restricciones.Last();
                        var rtMin = restricciones.First();
                        if(cantDias < rtMin.Minimo)
                        {
                            //cantDias = rtMin.Minimo;
                            DiasRestantes -= cantDias;
                            rt = rtMin;
                        }
                        else if(cantDias > rtMax.Maximo)
                        {
                            cantDias = rtMax.Maximo;
                            DiasRestantes -= cantDias;
                            rt = rtMax;
                        }                     
                        
                        ov.PrecioOrden += cantDias * _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == rt.RestriccionesId).Precio;
                        ov.PrecioOrden += cantDias * p.Seguro;
                    }
                }
                else
                {

                    if (buscador.FechaRecogida < rf.FechaInicio && rf.FechaFin < buscador.FechaEntrega)
                    {
                        //Si el rango esta incluido en el rango de recogida y entrega la cantidad de dias sera la diferencia del rango de fecha
                        cantDias = (rf.FechaFin - rf.FechaInicio).Days;
                        foreach (var item in restricciones)// se evalua por restricciones el valor de la cantidad de dias
                        {
                            
                            if (item.Minimo <= cantDias && cantDias <= item.Maximo)// si coincide la cantidad de dias con el rango de una restriccion se calcula
                            {
                                rt = item;
                                ov.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio * cantDias;
                                DiasRestantes -= cantDias; // se descuentan los dias que han sido incluidos en el precio
                                ov.PrecioOrden += cantDias * p.Seguro;
                                encontroRangoValido = true;
                                break;
                            }
                        }
                        if (!encontroRangoValido && restricciones.Any())
                        {
                            var rtMax = restricciones.Last();
                            var rtMin = restricciones.First();
                            if (cantDias < rtMin.Minimo)
                            {
                                //cantDias = rtMin.Minimo;
                                DiasRestantes -= cantDias;
                                rt = rtMin;
                            }
                            else if (cantDias > rtMax.Maximo)
                            {
                                cantDias = rtMax.Maximo;
                                DiasRestantes -= cantDias;
                                rt = rtMax;
                            }

                            ov.PrecioOrden += cantDias * _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == rt.RestriccionesId).Precio;
                            ov.PrecioOrden += cantDias * p.Seguro;
                        }

                    }
                    else
                   if (rf.FechaInicio < buscador.FechaRecogida && buscador.FechaRecogida <= rf.FechaFin)
                    {
                        //Si solo la fecha de recogida cae en rango la cantidad de dias sera la diferencia respecto al fin del rango
                        cantDias = (rf.FechaFin - buscador.FechaRecogida).Days + 1;
                        foreach (var item in restricciones)// se evalua por restricciones el valor de la cantidad de dias
                        {
                            if (item.Minimo <= cantDias && cantDias <= item.Maximo)// si coincide la cantidad de dias con el rango de una restriccion se calcula
                            {
                                rt = item;
                                ov.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio * cantDias;
                                ov.PrecioOrden += cantDias * p.Seguro;
                                DiasRestantes -= cantDias; // se descuentan los dias que han sido incluidos en el precio
                                encontroRangoValido = true;
                                break;
                            }
                        }
                        if (!encontroRangoValido && restricciones.Any())
                        {
                            var rtMax = restricciones.Last();
                            var rtMin = restricciones.First();
                            if (cantDias < rtMin.Minimo)
                            {
                                //cantDias = rtMin.Minimo;
                                DiasRestantes -= cantDias;
                                rt = rtMin;
                            }
                            else if (cantDias > rtMax.Maximo)
                            {
                                cantDias = rtMax.Maximo;
                                DiasRestantes -= cantDias;
                                rt = rtMax;
                            }

                            ov.PrecioOrden += cantDias * _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == rt.RestriccionesId).Precio;
                            ov.PrecioOrden += cantDias * p.Seguro;
                        }
                    }
                    else
                   if (rf.FechaFin >= buscador.FechaEntrega && buscador.FechaEntrega >= rf.FechaInicio)
                    {
                        //Si solo la fecha de Entrega cae en rango la cantidad de dias sera la diferencia respecto al fin del rango
                        cantDias = (buscador.FechaEntrega - rf.FechaInicio).Days;
                        foreach (var item in restricciones)// se evalua por restricciones el valor de la cantidad de dias
                        {
                            
                            if (item.Minimo <= cantDias && cantDias <= item.Maximo)// si coincide la cantidad de dias con el rango de una restriccion se calcula
                            {
                                rt = item;
                                ov.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio * cantDias;
                                ov.PrecioOrden += cantDias * p.Seguro;
                                DiasRestantes -= cantDias; // se descuentan los dias que han sido incluidos en el precio
                                encontroRangoValido = true;
                                break;
                            }
                        }
                        if (!encontroRangoValido && restricciones.Any())
                        {
                            var rtMax = restricciones.Last();
                            var rtMin = restricciones.First();
                            if (cantDias < rtMin.Minimo)
                            {
                                //cantDias = rtMin.Minimo;
                                DiasRestantes -= cantDias;
                                rt = rtMin;
                            }
                            else if (cantDias > rtMax.Maximo)
                            {
                                cantDias = rtMax.Maximo;
                                DiasRestantes -= cantDias;
                                rt = rtMax;
                            }

                            ov.PrecioOrden += cantDias * _context.RestriccionesPrecios.First(x => x.ProductoId == v.ProductoId && x.RestriccionesId == rt.RestriccionesId).Precio;
                            ov.PrecioOrden += cantDias * p.Seguro;
                        }
                    }


                }






                i++;
            }
        }



        // Post: api/Vehiculoes/Activar
        [HttpPost]
        [Route("Activar")]
        public async Task<IActionResult> PostAcivarVehiculo([FromBody] Vehiculo ve)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Vehiculo v = _context.Vehiculos.Single(x=>x.ProductoId==ve.ProductoId);
           if (ve.IsActivo)
            if(!_context.PrecioRentaAutos.Any(x=>x.ProductoId==v.ProductoId) ||
               !_context.RestriccionesPrecios.Any(x => x.ProductoId == v.ProductoId && x.Precio > 0)){

                return CreatedAtAction("ActivarVehiculo", new { id = -1, error = "Este producto no está listo para activar. Revise los precios" }, new { id = -1, error = "Este producto no está listo para activar. Revise los precios" });
            }
            v.IsActivo = ve.IsActivo;


            _context.Entry(v).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoExists(v.ProductoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVehiculo", new { id = v.ProductoId }, v);
        }



        // GET: api/Vehiculoes/BuscarOrden
        [HttpPost]
        [Route("CambiarPrecio")]
        public OrdenVehiculo GetOrdenVehiculosEspecificar([FromBody] BuscadorVehiculo buscador, int pageIndex = 1, int pageSize = 1)
        {
            // TODO agregar el calculo de la orden teniendo en  cuenta

            /// surcharger

            List<OrdenVehiculo> lista = new List<OrdenVehiculo>(); //Lista  a devolver (candidatos)
            Vehiculo vehi = _context.Vehiculos.Include(x => x.ListaDistribuidoresProducto).First(x => x.ProductoId == buscador.ProductoId);

            //Para saber que autos entran en la categoria pasada por parametros
            // List<VehiculoCategoriaAuto> cats = _context.VehiculoCategoriaAuto.Where(x => x.CategoriaAuto.CategoriaAutoId == buscador.CategoriaAuto.CategoriaAutoId).ToList();

            //Se buscan todos los auto con la transmision pasada por parametros
            List <Vehiculo> vehiculos = new List<Vehiculo>();
            vehiculos.Add(vehi);


            foreach (var v in vehiculos) //Se recorren los vehiculos que coinciden con el tipo de transmision
            {
                v.ListaDistribuidoresProducto = v.ListaDistribuidoresProducto.Where(x => x.DistribuidorId == buscador.DistribuidorId).ToList();
                foreach (var dist in v.ListaDistribuidoresProducto)
                {

                    OrdenVehiculo ov = new OrdenVehiculo();
                    int cantDiasGenenarl = (buscador.FechaEntrega - buscador.FechaRecogida).Days; //Cant. de dias a reservar
                    int cantDias = 0; // auxilar para rangos
                    int DiasRestantes = cantDiasGenenarl; // para saber que cantidad de dias son extra a las restricciones
                    ov.FechaRecogida = buscador.FechaRecogida;
                    ov.FechaEntrega = buscador.FechaEntrega;
                    Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar

                    ov.Vehiculo = v;
                    bool agregarOrden = true;

                    List<PrecioRentaAutos> precios = _context.PrecioRentaAutos.Include(x => x.Temporada.ListaFechasTemporada)
                    .Include(x => x.Temporada.Contrato.Distribuidor)
                    .Where(x => x.ProductoId == v.ProductoId && x.Temporada.Contrato.Distribuidor.DistribuidorId == dist.DistribuidorId).ToList();
                    PrecioRentaAutos ultimoPrecio = new PrecioRentaAutos();
                    if (!precios.Any())
                        continue;
                    ov.ListaPreciosRentaAutos = new List<OrdenVehiculoPrecioRentaAuto>();
                    foreach (var p in precios)
                    {


                        if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.FechaRecogida && buscador.FechaRecogida <= x.FechaFin) ||
                         (x.FechaFin >= buscador.FechaEntrega && buscador.FechaEntrega >= x.FechaInicio))) // si la fecha buscada esta en el rango de precios
                        {
                            ultimoPrecio = p;

                            //Se obtienen las restricciones ordenadas por el valor maximo de dias para calcular precio segun cantidad de dias
                            List<Restricciones> restricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == p.Temporada.TemporadaId).OrderBy(x => x.Minimo).ToList();
                            OrdenVehiculoPrecioRentaAuto ovpra = new OrdenVehiculoPrecioRentaAuto();

                            ovpra.PrecioRentaAutos = p;
                            ov.ListaPreciosRentaAutos.Add(ovpra);
                            ov.Distribuidor = p.Temporada.Contrato.Distribuidor;
                            try
                            {

                                switch (p.Temporada.Contrato.FormaCobro)// 2 - por dia 1 - PrimeraTemp 3 - UltimaTemp
                                {
                                    case 2:
                                        {
                                            Met_CalcularPrecioAutoPorDia(buscador, v, p, ov, cantDiasGenenarl, ref cantDias, ref DiasRestantes, restricciones);
                                            break;
                                        }
                                    case 1:
                                        {
                                            Met_CalcularPrecioAutoPorPrimeraTemporada(buscador, v, p, ov, cantDiasGenenarl, ref cantDias, ref DiasRestantes, restricciones);

                                            break;
                                        }
                                    case 3:
                                        {
                                            Met_CalcularPrecioAutoPorSegundaTemporada(buscador, v, p, ov, cantDiasGenenarl, ref cantDias, ref DiasRestantes, restricciones);
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }

                            }
                            catch
                            {
                                agregarOrden = false;
                                break;
                            }



                        }
                        if (DiasRestantes == 0)
                            break;
                    }
                    if (!agregarOrden)
                        continue;
                    ov.PrecioOrden += (DiasRestantes * ultimoPrecio.DiasExtra); //+ (cantDiasGenenarl * ultimoPrecio.Seguro);
                    ov.PrecioOrden += (DiasRestantes * ultimoPrecio.Seguro);



                    List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.VEHICLE).ToList();

                    foreach (Sobreprecio s in sobreprecios)
                    {
                        if (s.PrecioDesde <= ov.PrecioOrden && ov.PrecioOrden <= s.PrecioHasta)
                        {
                            ov.Sobreprecio = s;
                            decimal valorAplicado = 0;
                            if (s.PagoPorDia)
                            {
                                if (s.ValorDinero != null)
                                {
                                    valorAplicado = cantDiasGenenarl * (decimal)s.ValorDinero;
                                    ov.PrecioOrden += valorAplicado + (valorAplicado * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplicado = cantDiasGenenarl * ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    ov.PrecioOrden += valorAplicado + (valorAplicado * c.Descuento / 100);
                                }

                            }
                            else
                            {

                                if (s.ValorDinero != null)
                                {
                                    valorAplicado = (decimal)s.ValorDinero;
                                    ov.PrecioOrden += valorAplicado + (valorAplicado * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplicado = ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    ov.PrecioOrden += valorAplicado + (valorAplicado * c.Descuento / 100);
                                }

                            }
                            ov.ValorSobreprecioAplicado = valorAplicado;
                            break;
                        }

                    }

                    if (agregarOrden)
                        lista.Add(ov);


                }



            }


            if (lista != null && lista.Any())
                return lista.OrderByDescending(x => x.PrecioOrden).ToPagedList(pageIndex, pageSize).ToList()[0];
            else
            {
                return new OrdenVehiculo();
            }
           

        }



        // GET: api/Vehiculoes/RangosRestricciones
        [HttpPost]
        [Route("RangosRestricciones")]
        public Restricciones GetRangosRestricciones(DateTime fecha)
        {
            Restricciones result = null;
            var nombreTipoProduto=ValoresAuxiliares.VEHICLE;
            
            List<RangoFechas> rangosContienenFecha = _context.RangoFechas
                .Where(x =>  x.FechaInicio <= fecha && fecha <= x.FechaFin).ToList();

            foreach(var rangos in rangosContienenFecha)
            {
                Restricciones temporal = new Restricciones();
                List<Restricciones> restricciones = _context.Restricciones.Where(x =>x.Temporada.Contrato.TipoProducto.Nombre==nombreTipoProduto && x.Temporada.TemporadaId == rangos.TemporadaId).ToList();
                if (restricciones != null && restricciones.Any())
                {
                    temporal.Minimo = restricciones.OrderBy(x => x.Minimo).First().Minimo;
                    temporal.Maximo = restricciones.OrderBy(x => x.Maximo).Last().Maximo;
                }
                else
                {
                    continue;
                }
               
                if(result == null)
                {
                    result = temporal;
                }
                else
                {
                    if(temporal.Minimo < result.Minimo)
                    {
                        result.Minimo = temporal.Minimo;
                    }
                    if (temporal.Maximo > result.Maximo)
                    {
                        result.Maximo = temporal.Maximo;
                    }
                }
                


            }

                       
            return result;
        }



    }
}