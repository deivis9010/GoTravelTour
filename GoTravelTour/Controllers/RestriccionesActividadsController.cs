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
    public class RestriccionesActividadsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RestriccionesActividadsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/RestriccionesActividads
        [HttpGet]
        public IEnumerable<RestriccionesActividad> GetRestriccionesActividades(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<RestriccionesActividad> lista;
            if (col == "-1")
            {
                return _context.RestriccionesActividades
                    .Include(a => a.Temporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Actividad)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.RestriccionesActividades
                    .Include(a => a.Temporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Actividad)
                    .Where(p => (p.Actividad.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.RestriccionesActividades
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
        // GET: api/RestriccionesActividads/Count
        [Route("Count")]
        [HttpGet]
        public int GetRestriccionesActividadesCount()
        {
            return _context.RestriccionesActividades.Count();
        }

        // GET: api/RestriccionesActividads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestriccionesActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restriccionesActividad = await _context.RestriccionesActividades.FindAsync(id);

            if (restriccionesActividad == null)
            {
                return NotFound();
            }

            return Ok(restriccionesActividad);
        }

        // PUT: api/RestriccionesActividads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestriccionesActividad([FromRoute] int id, [FromBody] RestriccionesActividad restriccionesActividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restriccionesActividad.RestriccionesActividadId)
            {
                return BadRequest();
            }

            _context.Entry(restriccionesActividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestriccionesActividadExists(id))
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

        // POST: api/RestriccionesActividads
        [HttpPost]
        public async Task<IActionResult> PostRestriccionesActividad([FromBody] RestriccionesActividad restriccionesActividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RestriccionesActividades.Add(restriccionesActividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRestriccionesActividad", new { id = restriccionesActividad.RestriccionesActividadId }, restriccionesActividad);
        }

        // DELETE: api/RestriccionesActividads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestriccionesActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restriccionesActividad = await _context.RestriccionesActividades.FindAsync(id);
            if (restriccionesActividad == null)
            {
                return NotFound();
            }

            _context.RestriccionesActividades.Remove(restriccionesActividad);
            await _context.SaveChangesAsync();

            return Ok(restriccionesActividad);
        }

        private bool RestriccionesActividadExists(int id)
        {
            return _context.RestriccionesActividades.Any(e => e.RestriccionesActividadId == id);
        }
    }
}