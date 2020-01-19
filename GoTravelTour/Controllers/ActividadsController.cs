using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;


namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ActividadsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Actividads
        [HttpGet]
        public IEnumerable<Actividad> GetActividadess(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Actividad> lista;
            if (col == "-1")
            {
                lista= _context.Actividadess
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.ServiciosAdicionados)
                    .Include(a => a.Region)
                    .OrderBy(a=>a.Nombre)
                    .ToList();
                
                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Actividadess
                    .Include(a => a.ListaComodidades)
                    .Include(v => v.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.ServiciosAdicionados)
                    .Include(a => a.Region)
                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
              
            }
            else
            {
                lista = _context.Actividadess
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.ServiciosAdicionados)
                    .Include(a => a.Region)
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
        // GET: api/Actividads/Count
        [Route("Count")]
        [HttpGet]
        public int GetActividadsCount()
        {
            return _context.Actividadess.Count();
        }


        // GET: api/Actividads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var actividad = await _context.Actividadess.FindAsync(id);

            if (actividad == null)
            {
                return NotFound();
            }

            return Ok(actividad);
        }

        // PUT: api/Actividads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActividad([FromRoute] int id, [FromBody] Actividad actividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != actividad.ProductoId)
            {
                return BadRequest();
            }
            if (_context.Actividadess.Any(c => c.Nombre == actividad.Nombre && actividad.ProductoId != id))
            {
                return CreatedAtAction("GetActividades", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            actividad.Region = _context.Regiones.First(x => x.RegionId == actividad.Region.RegionId);

            _context.Entry(actividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActividadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetActividad", new { id = actividad.ProductoId }, actividad);
        }

        // POST: api/Actividads
        [HttpPost]
        public async Task<IActionResult> PostActividad([FromBody] Actividad actividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Actividadess.Any(c => c.Nombre == actividad.Nombre))
            {
                return CreatedAtAction("GetActividades", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            actividad.SKU = u.GetSKUCodigo();

            actividad.Region = _context.Regiones.First(f => f.RegionId == actividad.Region.RegionId);
            if(actividad.ServiciosAdicionados != null && actividad.ServiciosAdicionados.Count() > 0)
            {
                int i = 0;
                while(i < actividad.ServiciosAdicionados.Count())
                {
                    actividad.ServiciosAdicionados[i] = _context.Servicio.First(ser => ser.ServicioId == actividad.ServiciosAdicionados[i].ServicioId);
                    i++;
                }

            }
            _context.Actividadess.Add(actividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActividad", new { id = actividad.ProductoId }, actividad);
        }

        // DELETE: api/Actividads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var actividad = await _context.Actividadess.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            _context.Actividadess.Remove(actividad);
            await _context.SaveChangesAsync();

            return Ok(actividad);
        }

        private bool ActividadExists(int id)
        {
            return _context.Actividadess.Any(e => e.ProductoId == id);
        }

        // GET: api/Actividads/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Contrato> GetContratosByFiltros(int idContrato = -1, int idDistribuidor = -1)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).Where(x => x.DistribuidorId == contrato.DistribuidorId).ToList();
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioServicioActividad = _context.Servicio
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
               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor)
               .OrderBy(a => a.Nombre)
               .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).Where(x => x.DistribuidorId == contrato.DistribuidorId).ToList();
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioServicioActividad = _context.Servicio
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
                .Where(a => a.ContratoId == idContrato)
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).Where(x => x.DistribuidorId == contrato.DistribuidorId).ToList();
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioServicioActividad = _context.Servicio
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
                .Where(a => a.DistribuidorId == idDistribuidor)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).Where(x => x.DistribuidorId == contrato.DistribuidorId).ToList();
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioServicioActividad = _context.Servicio
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