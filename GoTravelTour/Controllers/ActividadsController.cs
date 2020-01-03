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
                return _context.Actividadess
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.Comodidades)
                    .OrderBy(a=>a.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Actividadess
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.Comodidades)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Actividadess
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.Comodidades)
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

            return NoContent();
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
    }
}