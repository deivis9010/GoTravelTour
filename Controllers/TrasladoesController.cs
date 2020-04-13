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
        public IEnumerable<Traslado> GetTraslados(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor=0)
        {
            IEnumerable<Traslado> lista;
            if (col == "-1")
            {
                return _context.Traslados
                    .Include(x=> x.Proveedor)
                    /*.Include(x => x.TipoProducto)
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)*/
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
                    .Include(a => a.TipoTransporte)
                    //.Include(a => a.ListaDistribuidoresProducto)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Traslados
                    .Include(x => x.Proveedor)
                   /* .Include(x => x.TipoProducto)*/
                    .Include(a => a.TipoTransporte)
                    /*.Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)*/
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
            //if (_context.Traslados.Any(c => c.Nombre == traslado.Nombre && traslado.TrasladoId != id))
            //{
            //    return CreatedAtAction("GetTraslado", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            //}

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
                if(traslado.PuntoInteres!=null && traslado.PuntoInteres.PuntoInteresId > 0)
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
            //if (_context.Traslados.Any(c => c.Nombre == traslado.Nombre))
            //{
            //    return CreatedAtAction("GetTraslado", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            //}
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



        // GET: api/Trasladoes/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Contrato> GetContratosByFiltros(int idContrato = -1, int idDistribuidor = -1, int idProveedor=0 , int idProducto=0)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.TipoProducto.Nombre == "Transportation")
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {
                        if (idProducto==0)
                        {
                            contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                           .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Transportation" && x.Producto.ProveedorId == idProveedor).ToList();
                        }
                        else
                        {
                            contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                           .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Transportation" && x.Producto.ProveedorId == idProveedor && x.Producto.ProductoId== idProducto).ToList();
                        }
                       
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
               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == "Transportation")
               .OrderBy(a => a.Nombre)
               .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        if (idProducto == 0)
                        {
                            contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                           .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Transportation" && x.Producto.ProveedorId == idProveedor).ToList();
                        }
                        else
                        {
                            contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                           .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Transportation" && x.Producto.ProveedorId == idProveedor && x.Producto.ProductoId == idProducto).ToList();
                        }
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
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == "Transportation")
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        if (idProducto == 0)
                        {
                            contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                           .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Transportation" && x.Producto.ProveedorId == idProveedor).ToList();
                        }
                        else
                        {
                            contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                           .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Transportation" && x.Producto.ProveedorId == idProveedor && x.Producto.ProductoId == idProducto).ToList();
                        }

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
                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == "Transportation")
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        if (idProducto == 0)
                        {
                            contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                           .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Transportation" && x.Producto.ProveedorId == idProveedor).ToList();
                        }
                        else
                        {
                            contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                           .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Transportation" && x.Producto.ProveedorId == idProveedor && x.Producto.ProductoId == idProducto).ToList();
                        }

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

    }
}