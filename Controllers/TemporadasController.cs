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

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporadasController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TemporadasController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Temporadas
        [HttpGet]
        public IEnumerable<Temporada> GetTemporadas(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Temporada> lista;
            if (col == "-1")
            {
                lista = _context.Temporadas
                    .Include(a => a.ListaFechasTemporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Contrato.Distribuidor)
                    .Include(a => a.Contrato.TipoProducto)
                    .OrderBy(a => a.Nombre)

                    .ToList();
                if (lista.Count() > 0 )
                    foreach ( var temp in lista)
                {
                    if(ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                    temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                   
                }
                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Temporadas
                    .Include(a => a.ListaFechasTemporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Contrato.Distribuidor)
                    .Include(a => a.Contrato.TipoProducto)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Temporadas
                    .Include(a => a.ListaFechasTemporada)
                    .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
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
        // GET: api/Temporadas/Count
        [Route("Count")]
        [HttpGet]
        public int GetTemporadasCount()
        {
            return _context.Temporadas.Count();
        }

        // GET: api/Temporadas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemporada([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var temporada = await _context.Temporadas.FindAsync(id);

            if (temporada == null)
            {
                return NotFound();
            }

            return Ok(temporada);
        }

        // PUT: api/Temporadas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemporada([FromRoute] int id, [FromBody] Temporada temporada)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != temporada.TemporadaId)
            {
                return BadRequest();
            }

            _context.Entry(temporada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemporadaExists(id))
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

        // POST: api/Temporadas
        [HttpPost]
        public async Task<IActionResult> PostTemporada([FromBody] Temporada temporada)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Temporadas.Add(temporada);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTemporada", new { id = temporada.TemporadaId }, temporada);
        }

        // DELETE: api/Temporadas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemporada([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var temporada = await _context.Temporadas.FindAsync(id);
            if (temporada == null)
            {
                return NotFound();
            }

            _context.Temporadas.Remove(temporada);
            await _context.SaveChangesAsync();

            return Ok(temporada);
        }

        private bool TemporadaExists(int id)
        {
            return _context.Temporadas.Any(e => e.TemporadaId == id);
        }


        // GET: api/Temporadas/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Temporada> GetTemporadasByFiltros(int idContrato = -1, int idDistribuidor = -1, int idTipoProducto = -1 , int idProveedor=0)
        {
            IEnumerable<Temporada> lista;
            
            if (idContrato == -1 &&  idDistribuidor == -1 && idTipoProducto == -1)
            {
                lista= _context.Temporadas
                .Include(a => a.ListaFechasTemporada)
                .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)

                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var temp in lista)
                    {
                        if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                            temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                        if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                        {
                            temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                                .Include(x=>x.Producto)
                                .Where(x => x.Producto.ProveedorId == idProveedor && x.DistribuidorId == idDistribuidor && x.Producto.TipoProducto.Nombre== ValoresAuxiliares.ACCOMMODATION).ToList();

                            temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                        }



                    }
                return lista;
            } else
              if (idContrato != -1 && idDistribuidor != -1 && idTipoProducto != -1)
            {
                lista= _context.Temporadas
                .Include(a => a.ListaFechasTemporada)
                .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
                .Where(a => a.ContratoId == idContrato && a.Contrato.TipoProductoId == idTipoProducto && a.Contrato.DistribuidorId == idDistribuidor)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var temp in lista)
                    {
                        if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                            temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();

                        if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                        {
                            temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                                .Include(x => x.Producto)
                                .Where(x => x.Producto.ProveedorId == idProveedor && x.DistribuidorId == idDistribuidor && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).ToList();
                            temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                        }
                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor != -1 && idTipoProducto == -1)
            {
                lista= _context.Temporadas
                .Include(a => a.ListaFechasTemporada)
                .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
                .Where(a => a.ContratoId == idContrato && a.Contrato.DistribuidorId == idDistribuidor)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var temp in lista)
                    {
                        if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                            temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                        if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                        {
                            temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                               .Include(x=>x.Producto).Where(x => x.Producto.ProveedorId == idProveedor && x.DistribuidorId == idDistribuidor && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).ToList();
                            temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                        }
                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor == -1 && idTipoProducto != -1)
            {
                lista= _context.Temporadas
                .Include(a => a.ListaFechasTemporada)
                .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
                .Where(a => a.ContratoId == idContrato && a.Contrato.TipoProductoId == idTipoProducto)
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var temp in lista)
                    {
                        if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                            temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                        if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                        {
                            temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                                .Include(x => x.Producto)
                                .Where(x => x.Producto.ProveedorId == idProveedor && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).ToList();
                            temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                        }
                    }
                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor != -1 && idTipoProducto != -1)
            {
                lista= _context.Temporadas
                .Include(a => a.ListaFechasTemporada)
                .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
                .Where(a => a.Contrato.DistribuidorId == idDistribuidor && a.Contrato.TipoProductoId == idTipoProducto)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var temp in lista)
                    {
                        if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                            temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                        if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                        {
                            temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                                .Include(x => x.Producto)
                                .Where(x => x.Producto.ProveedorId == idProveedor && x.DistribuidorId == idDistribuidor && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).ToList();
                            temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                        }
                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor == -1 && idTipoProducto == -1)
            {
                lista= _context.Temporadas
                .Include(a => a.ListaFechasTemporada)
                .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
                .Where(a => a.ContratoId == idContrato)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var temp in lista)
                    {
                        if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                            temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                        if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                        {
                            temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                                .Include(x => x.Producto)
                                .Where(x => x.Producto.ProveedorId == idProveedor && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).ToList();
                            temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                        }
                    }
                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor != -1 && idTipoProducto == -1)
            {
                lista= _context.Temporadas
                .Include(a => a.ListaFechasTemporada)
                .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
                .Where(a => a.Contrato.DistribuidorId == idDistribuidor)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var temp in lista)
                    {
                        if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                            temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                        if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                        {
                            temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                                .Include(x => x.Producto)
                                .Where(x => x.Producto.ProveedorId == idProveedor && x.DistribuidorId==idDistribuidor && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).ToList();
                            temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                        }
                    }
                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor == -1 && idTipoProducto != -1)
            {
                lista= _context.Temporadas
                .Include(a => a.ListaFechasTemporada)
                .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
                .Where(a => a.Contrato.TipoProductoId == idTipoProducto)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var temp in lista)
                    {
                        if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                            temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                        if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                        {
                            temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                                .Include(x => x.Producto)
                                .Where(x => x.Producto.ProveedorId == idProveedor && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).ToList();
                            temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                        }
                    }
                return lista;
            }

            lista= _context.Temporadas
               .Include(a => a.ListaFechasTemporada)
               .Include(a => a.Contrato)
                .Include(a => a.Contrato.Distribuidor)
                .Include(a => a.Contrato.TipoProducto)
               .OrderBy(a => a.Nombre)
               .ToList();
            if (lista.Count() > 0)
                foreach (var temp in lista)
                {
                    if (ValoresAuxiliares.ACTIVITY.Equals(temp.Contrato.TipoProducto.Nombre) || ValoresAuxiliares.VEHICLE.Equals(temp.Contrato.TipoProducto.Nombre))
                        temp.ListaRestricciones = _context.Restricciones.Include(x => x.Temporada).Where(x => x.Temporada.TemporadaId == temp.TemporadaId).ToList();
                    if (ValoresAuxiliares.ACCOMMODATION.Equals(temp.Contrato.TipoProducto.Nombre))
                    {
                        temp.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                            .Include(x => x.Producto)
                            .Where(x => x.Producto.ProveedorId == idProveedor && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).ToList();
                        temp.ListaFechasTemporada.ForEach(x => x = _context.RangoFechas.Include(r => r.Producto).First(r => r.RangoFechasId == x.RangoFechasId));

                    }
                }
            return lista;



        }
    }
}