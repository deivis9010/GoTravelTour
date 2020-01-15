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
    public class PrecioActividadsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PrecioActividadsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PrecioActividads
        [HttpGet]
        public IEnumerable<PrecioActividad> GetPrecioActividades(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<PrecioActividad> lista;
            if (col == "-1")
            {
                return _context.PrecioActividades
                    .Include(a => a.Temporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Actividad)
                    .OrderBy(a => a.Actividad.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PrecioActividades
                    .Include(a => a.Temporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Actividad)
                    .Where(p => (p.Actividad.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.PrecioActividades
                    .Include(a => a.Temporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Actividad)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Actividad.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Actividad.Nombre);

                        }
                    }

                    break;
            }

            return lista;
        }

        // GET: api/PrecioActividads/Count
        [Route("Count")]
        [HttpGet]
        public int GetPrecioActividadsCount()
        {
            return _context.PrecioActividades.Count();
        }

        // GET: api/PrecioActividads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrecioActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioActividad = await _context.PrecioActividades.FindAsync(id);

            if (precioActividad == null)
            {
                return NotFound();
            }

            return Ok(precioActividad);
        }

        // PUT: api/PrecioActividads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecioActividad([FromRoute] int id, [FromBody] PrecioActividad precioActividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != precioActividad.PrecioActividadId)
            {
                return BadRequest();
            }
            precioActividad.Contrato = _context.Contratos.First(x => x.ContratoId == precioActividad.Contrato.ContratoId);
            precioActividad.Temporada = _context.Temporadas.First(x => x.TemporadaId == precioActividad.Temporada.TemporadaId);
            _context.Entry(precioActividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioActividadExists(id))
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

        // POST: api/PrecioActividads
        [HttpPost]
        public async Task<IActionResult> PostPrecioActividad([FromBody] PrecioActividad precioActividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            precioActividad.Contrato = _context.Contratos.First(x => x.ContratoId == precioActividad.Contrato.ContratoId);
            precioActividad.Temporada = _context.Temporadas.First(x => x.TemporadaId == precioActividad.Temporada.TemporadaId);
            _context.PrecioActividades.Add(precioActividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrecioActividad", new { id = precioActividad.PrecioActividadId }, precioActividad);
        }

        // DELETE: api/PrecioActividads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecioActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioActividad = await _context.PrecioActividades.FindAsync(id);
            if (precioActividad == null)
            {
                return NotFound();
            }

            _context.PrecioActividades.Remove(precioActividad);
            await _context.SaveChangesAsync();

            return Ok(precioActividad);
        }

        private bool PrecioActividadExists(int id)
        {
            return _context.PrecioActividades.Any(e => e.PrecioActividadId == id);
        }
    }
}