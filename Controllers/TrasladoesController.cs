using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using Microsoft.AspNetCore.Authorization;
using GoTravelTour.Utiles;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrasladoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TrasladoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Trasladoes
        [HttpGet]
        public IEnumerable<Traslado> GetTraslados(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<Traslado> lista;
            if (col == "-1")
            {
                return _context.Traslados
                    .Include(x => x.Proveedor)
                    /*.Include(x => x.TipoProducto)
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)*/
                    .Include(a => a.PuntoInteres)
                    .Include(a => a.TipoTransporte)
                     .Where(x => x.ProveedorId == idProveedor)
                    .OrderBy(a => a.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Traslados
                    .Include(x => x.Proveedor)
                    /*.Include(x => x.TipoProducto)
                    .Include(a => a.ListaComodidades)*/
                    .Include(a => a.PuntoInteres)
                    .Include(a => a.TipoTransporte)
                    //.Include(a => a.ListaDistribuidoresProducto)
                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Traslados
                    .Include(x => x.Proveedor)
                   /* .Include(x => x.TipoProducto)*/
                   .Include(a => a.PuntoInteres)
                    .Include(a => a.TipoTransporte)
                    /*.Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)*/
                    .OrderBy(a => a.Nombre)
                    .ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/Trasladoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetTrasladosCount()
        {
            return _context.Traslados.Count();
        }


        // GET: api/Trasladoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTraslado([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /* var traslado = await _context.Traslados
                 .FindAsync(id);*/
            var traslado = _context.Traslados
           .Include(x => x.Proveedor)
               .Include(x => x.TipoProducto)
               .Include(a => a.ListaComodidades)
               .Include(a => a.ListaDistribuidoresProducto)
               .Include(a => a.PuntoInteres)
               .Include(a => a.TipoTransporte)

           .Single(x => x.ProductoId == id);

            if (traslado == null)
            {
                return NotFound();
            }

            return Ok(traslado);
        }

        // PUT: api/Trasladoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTraslado([FromRoute] int id, [FromBody] Traslado traslado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != traslado.ProductoId)
            {
                return BadRequest();
            }
            if (_context.Traslados.Any(c => c.Nombre == traslado.Nombre && c.TrasladoId != id && c.ProveedorId == traslado.ProveedorId))
            {
                return CreatedAtAction("GetTraslado", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            List<ComodidadesProductos> comodidades = _context.ComodidadesProductos.Where(x => x.ProductoId == traslado.ProductoId).ToList();
            foreach (var item in comodidades)
            {
                _context.ComodidadesProductos.Remove(item);
            }

            List<ProductoDistribuidor> distribuidors = _context.ProductoDistribuidores.Where(x => x.ProductoId == traslado.ProductoId).ToList();
            foreach (var item in distribuidors)
            {
                _context.ProductoDistribuidores.Remove(item);
            }
            if (traslado.ListaDistribuidoresProducto != null)
                foreach (var item in traslado.ListaDistribuidoresProducto)
                {
                    item.ProductoId = traslado.ProductoId;
                    _context.ProductoDistribuidores.Add(item);
                }
            if (traslado.ListaComodidades != null)
                foreach (var item in traslado.ListaComodidades)
                {
                    item.ProductoId = traslado.ProductoId;
                    _context.ComodidadesProductos.Add(item);
                }
            traslado.Proveedor = _context.Proveedores.First(x => x.ProveedorId == traslado.ProveedorId);
            if (traslado.ListaDistribuidoresProducto != null)
                if (traslado.PuntoInteres != null && traslado.PuntoInteres.PuntoInteresId > 0)
                    traslado.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == traslado.PuntoInteres.PuntoInteresId);
            traslado.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == traslado.TipoProductoId);
            _context.Entry(traslado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrasladoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

          
            return CreatedAtAction("GetTraslado", new { id = traslado.ProductoId }, traslado); ;
        }

        // POST: api/Trasladoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostTraslado([FromBody] Traslado traslado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Traslados.Any(c => c.Nombre == traslado.Nombre && c.ProveedorId == traslado.ProveedorId))
            {
                return CreatedAtAction("GetTraslado", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            traslado.SKU = u.GetSKUCodigo();
            traslado.Proveedor = _context.Proveedores.First(x => x.ProveedorId == traslado.ProveedorId);
            if (traslado.PuntoInteres != null && traslado.PuntoInteres.PuntoInteresId > 0)
                traslado.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == traslado.PuntoInteres.PuntoInteresId);
            traslado.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == traslado.TipoProductoId);
            _context.Traslados.Add(traslado);
            await _context.SaveChangesAsync();

           

            return CreatedAtAction("GetTraslado", new { id = traslado.ProductoId }, traslado);
        }

        // DELETE: api/Trasladoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTraslado([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var traslado = await _context.Traslados.FindAsync(id);
            if (traslado == null)
            {
                return NotFound();
            }

            _context.Traslados.Remove(traslado);
            await _context.SaveChangesAsync();

            return Ok(traslado);
        }

        private bool TrasladoExists(int id)
        {
            return _context.Traslados.Any(e => e.ProductoId == id);
        }


        // GET: api/Trasladoes/FiltrosCount
        [HttpGet]
        [Route("FiltrosCount")]
        public IEnumerable<Contrato> GetContratosByFiltrosCount(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos

                .Where(a => a.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
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

               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
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
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
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

                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
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




        // GET: api/Trasladoes/Filtros
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
                .Where(a => a.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
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
                                contrato.Temporadas[i].ListaPrecioTraslados = _context.PrecioTraslados
                                     .Include(x => x.Rutas)
                                    .Include(x => x.Rutas.PuntoInteresDestino)
                                    .Include(x => x.Rutas.PuntoInteresOrigen)
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
               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
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
                                contrato.Temporadas[i].ListaPrecioTraslados = _context.PrecioTraslados
                                     .Include(x => x.Rutas)
                                    .Include(x => x.Rutas.PuntoInteresDestino)
                                    .Include(x => x.Rutas.PuntoInteresOrigen)
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
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
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
                                contrato.Temporadas[i].ListaPrecioTraslados = _context.PrecioTraslados
                                    .Include(x => x.Rutas)
                                    .Include(x => x.Rutas.PuntoInteresDestino)
                                    .Include(x => x.Rutas.PuntoInteresOrigen)
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
                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
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
                                contrato.Temporadas[i].ListaPrecioTraslados = _context.PrecioTraslados

                                    .Include(x => x.Rutas)
                                    .Include(x => x.Rutas.PuntoInteresDestino)
                                    .Include(x => x.Rutas.PuntoInteresOrigen)
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
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
                .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION && x.ProductoId == idProducto)
                .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION && x.Producto.ProveedorId == idProveedor)
                .ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto)
                .ToPagedList(pageIndex, pageSize).ToList();
            }
        }

        private int CantidadProductoProveedor(int idProveedor, int idProducto, Contrato contrato)
        {

            if (idProducto == 0 && idProveedor == 0)
            {
                return _context.ProductoDistribuidores
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION)
                .Count();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                return _context.ProductoDistribuidores
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION && x.ProductoId == idProducto)
                .Count();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                return _context.ProductoDistribuidores
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION && x.Producto.ProveedorId == idProveedor)
                .Count();
            }
            else
            {
                return _context.ProductoDistribuidores
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto)
                .Count();
            }
        }


        // GET: api/Trasladoes/BuscarOrdenCount
        [HttpPost]
        [Route("BuscarOrdenCount")]
        public int GetOrdenTrasladosCount([FromBody] BuscadorTraslado buscador)
        {
            List<OrdenTraslado> lista = new List<OrdenTraslado>(); //Lista  a devolver (candidatos)


            //Se buscan todos los traslados con la transmision pasada por parametros
            List<Traslado> traslados = _context.Traslados.Include(d => d.ListaDistribuidoresProducto).Where(x => x.IsActivo && x.CapacidadTraslado >= buscador.CantidadPasajeros).ToList();


            foreach (var t in traslados)
            {
                //Se buscan las rutas que contienen el punto de origen y destino
                List<Rutas> posiblesRutas = _context.Rutas.Where(x => (x.PuntoInteresOrigen.PuntoInteresId == buscador.Origen.PuntoInteresId &&
                x.PuntoInteresDestino.PuntoInteresId == buscador.Destino.PuntoInteresId) ||
                (x.PuntoInteresOrigen.PuntoInteresId == buscador.Destino.PuntoInteresId &&
                x.PuntoInteresDestino.PuntoInteresId == buscador.Origen.PuntoInteresId)).ToList();

                foreach (var r in posiblesRutas)
                {

                    foreach (var dist in t.ListaDistribuidoresProducto)
                    {

                        //Se buscan los precios correspondientes entre ruta y traslado
                        List<PrecioTraslado> precios = _context.PrecioTraslados.Include(x => x.Temporada.ListaFechasTemporada)
                       .Include(x => x.Temporada.Contrato.Distribuidor)
                       .Where(x => x.ProductoId == t.ProductoId && x.RutasId == r.RutasId && x.Temporada.Contrato.DistribuidorId == dist.DistribuidorId).ToList();
                        foreach (var p in precios)
                        {
                            OrdenTraslado ov = new OrdenTraslado();
                            if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.Fecha && buscador.Fecha <= x.FechaFin))) // si la fecha buscada esta en el rango de precios
                            {
                                Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                                ov.PrecioTraslado = p;
                                ov.Distribuidor = p.Temporada.Contrato.Distribuidor;
                                ov.Traslado = t;
                                ov.FechaRecogida = buscador.Fecha;
                                ov.PuntoOrigen = buscador.Origen;
                                ov.PuntoDestino = buscador.Destino;
                                ov.PrecioOrden += p.Precio;


                                //Se aplica la ganancia correspondiente
                                List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION).ToList();

                                foreach (Sobreprecio s in sobreprecios)
                                {

                                    if (s.PrecioDesde <= ov.PrecioOrden && ov.PrecioOrden <= s.PrecioHasta)
                                    {
                                        ov.Sobreprecio = s;
                                        decimal valorAplicado = 0;
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

                                        ov.ValorSobreprecioAplicado = valorAplicado;
                                        break;
                                    }

                                }
                                ov.IsIdaVuelta = buscador.IsIdaVuelta;

                                if (ov.IsIdaVuelta) ov.PrecioOrden = 2 * ov.PrecioOrden;
                                if (ov.PrecioOrden > 0)
                                    lista.Add(ov);
                            }

                        }


                    }


                }



            }


            return lista.Count();

        }


        // GET: api/Trasladoes/BuscarOrden
        [HttpPost]
        [Route("BuscarOrden")]
        public List<OrdenTraslado> GetOrdenTraslados([FromBody] BuscadorTraslado buscador, int pageIndex = 1, int pageSize = 1000)
        {


            List<OrdenTraslado> lista = new List<OrdenTraslado>(); //Lista  a devolver (candidatos)


            //Se buscan todos los traslados con la transmision pasada por parametros
            List<Traslado> traslados = _context.Traslados.Include(d => d.ListaDistribuidoresProducto).Where(x => x.IsActivo && x.CapacidadTraslado >= buscador.CantidadPasajeros).ToList();


            foreach (var t in traslados)
            {
                //Se buscan las rutas que contienen el punto de origen y destino
                List<Rutas> posiblesRutas = _context.Rutas.Where(x => (x.PuntoInteresOrigen.PuntoInteresId == buscador.Origen.PuntoInteresId &&
                x.PuntoInteresDestino.PuntoInteresId == buscador.Destino.PuntoInteresId) ||
                (x.PuntoInteresOrigen.PuntoInteresId == buscador.Destino.PuntoInteresId &&
                x.PuntoInteresDestino.PuntoInteresId == buscador.Origen.PuntoInteresId)).ToList();

                foreach (var r in posiblesRutas)
                {

                    foreach (var dist in t.ListaDistribuidoresProducto)
                    {

                        //Se buscan los precios correspondientes entre ruta y traslado
                        List<PrecioTraslado> precios = _context.PrecioTraslados.Include(x => x.Temporada.ListaFechasTemporada)
                       .Include(x => x.Temporada.Contrato.Distribuidor)
                       .Where(x => x.ProductoId == t.ProductoId && x.RutasId == r.RutasId && x.Temporada.Contrato.DistribuidorId == dist.DistribuidorId).ToList();
                        foreach (var p in precios)
                        {
                            OrdenTraslado ov = new OrdenTraslado();
                            if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.Fecha && buscador.Fecha <= x.FechaFin))) // si la fecha buscada esta en el rango de precios
                            {
                                Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                                ov.PrecioTraslado = p;
                                ov.Distribuidor = p.Temporada.Contrato.Distribuidor;
                                ov.Traslado = t;
                                ov.FechaRecogida = buscador.Fecha;
                                ov.PuntoOrigen = buscador.Origen;
                                ov.PuntoDestino = buscador.Destino;
                                ov.PrecioOrden += p.Precio;


                                //Se aplica la ganancia correspondiente
                                List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION).ToList();

                                foreach (Sobreprecio s in sobreprecios)
                                {

                                    if (s.PrecioDesde <= ov.PrecioOrden && ov.PrecioOrden <= s.PrecioHasta)
                                    {
                                        ov.Sobreprecio = s;
                                        decimal valorAplicado = 0;
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

                                        ov.ValorSobreprecioAplicado = valorAplicado;
                                        break;
                                    }

                                }
                                ov.IsIdaVuelta = buscador.IsIdaVuelta;

                                if (ov.IsIdaVuelta) ov.PrecioOrden = 2 * ov.PrecioOrden;
                                if(ov.PrecioOrden > 0)
                                lista.Add(ov);
                            }

                        }


                    }


                }



            }


            return lista.OrderByDescending(x => x.PrecioOrden).ToPagedList(pageIndex, pageSize).ToList();


        }


        // Post: api/Trasladoes/Activar
        [HttpPost]
        [Route("Activar")]
        public async Task<IActionResult> PostAcivarTraslado([FromBody] Traslado tl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Traslado t = _context.Traslados.Single(x => x.ProductoId == tl.ProductoId);
            if (tl.IsActivo)
                if (!_context.PrecioTraslados.Any(x => x.ProductoId == t.ProductoId && x.Precio > 0))
                {
                    return CreatedAtAction("ActivarTraslado", new { id = -1, error = "Este producto no está listo para activar. Revise los precios" }, new { id = -1, error = "Este producto no está listo para activar. Revise los precios" });
                }

            _context.Entry(t).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrasladoExists(t.ProductoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTraslado", new { id = t.ProductoId }, t);
        }





        // GET: api/Trasladoes/BuscarOrden
        [HttpPost]
        [Route("CambiarPrecio")]
        public OrdenTraslado GetOrdenTrasladosEspecifico([FromBody] BuscadorTraslado buscador, int pageIndex = 1, int pageSize = 10)
        {


            List<OrdenTraslado> lista = new List<OrdenTraslado>(); //Lista  a devolver (candidatos)


            //Se buscan todos los traslados con la transmision pasada por parametros
            Traslado traslado = _context.Traslados.Include(d => d.ListaDistribuidoresProducto).Where(x => x.ProductoId == buscador.ProductoId).First();
            List<Traslado> traslados = new List<Traslado>();
            traslados.Add(traslado);

            foreach (var t in traslados)
            {
                //Se buscan las rutas que contienen el punto de origen y destino
                /* List<Rutas> posiblesRutas = _context.Rutas.Where(x => (x.PuntoInteresOrigen.PuntoInteresId == buscador.Origen.PuntoInteresId &&
                 x.PuntoInteresDestino.PuntoInteresId == buscador.Destino.PuntoInteresId) ||
                 (x.PuntoInteresOrigen.PuntoInteresId == buscador.Destino.PuntoInteresId &&
                 x.PuntoInteresDestino.PuntoInteresId == buscador.Origen.PuntoInteresId)).ToList();*/
                List<Rutas> posiblesRutas = _context.Rutas.Where(x => x.RutasId == buscador.RutaId).ToList();

                foreach (var r in posiblesRutas)
                {
                    t.ListaDistribuidoresProducto = t.ListaDistribuidoresProducto.Where(x => x.DistribuidorId == buscador.DistribuidorId).ToList();
                    foreach (var dist in t.ListaDistribuidoresProducto)
                    {

                        //Se buscan los precios correspondientes entre ruta y traslado
                        List<PrecioTraslado> precios = _context.PrecioTraslados.Include(x => x.Temporada.ListaFechasTemporada)
                       .Include(x => x.Temporada.Contrato.Distribuidor)
                       .Where(x => x.ProductoId == t.ProductoId && x.RutasId == r.RutasId && x.Temporada.Contrato.DistribuidorId == dist.DistribuidorId).ToList();
                        foreach (var p in precios)
                        {
                            OrdenTraslado ov = new OrdenTraslado();
                            if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.Fecha && buscador.Fecha <= x.FechaFin))) // si la fecha buscada esta en el rango de precios
                            {
                                Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                                ov.PrecioTraslado = p;
                                ov.Distribuidor = p.Temporada.Contrato.Distribuidor;
                                ov.Traslado = t;
                                ov.FechaRecogida = buscador.Fecha;
                                ov.PuntoOrigen = buscador.Origen;
                                ov.PuntoDestino = buscador.Destino;
                                ov.PrecioOrden += p.Precio;


                                //Se aplica la ganancia correspondiente
                                List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION).ToList();

                                foreach (Sobreprecio s in sobreprecios)
                                {

                                    if (s.PrecioDesde <= ov.PrecioOrden && ov.PrecioOrden <= s.PrecioHasta)
                                    {
                                        ov.Sobreprecio = s;
                                        decimal valorAplicado = 0;
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

                                        ov.ValorSobreprecioAplicado = valorAplicado;
                                        break;
                                    }

                                }
                                ov.IsIdaVuelta = buscador.IsIdaVuelta;

                                if (ov.IsIdaVuelta) ov.PrecioOrden = 2 * ov.PrecioOrden;
                                lista.Add(ov);
                            }

                        }


                    }


                }



            }

            if(lista != null && lista.Any())
            return lista.OrderByDescending(x => x.PrecioOrden).ToPagedList(pageIndex, pageSize).ToList()[0];
            else
            {
                return new OrdenTraslado();
            }


        }


    }
}