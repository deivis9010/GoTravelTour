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
    public class TipoServiciosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TipoServiciosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/TipoServicios
        [HttpGet]
        public IEnumerable<TipoServicio> GetTipoServicio(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<TipoServicio> lista;
            if (col == "-1")
            {
                return _context.TipoServicio
                    .OrderBy(a => a.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.TipoServicio
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.TipoServicio
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
        // GET: api/TipoServicios/Count
        [Route("Count")]
        [HttpGet]
        public int GetTipoServiciosCount()
        {
            return _context.TipoServicio.Count();
        }

        // GET: api/TipoServicios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoServicio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoServicio = await _context.TipoServicio.FindAsync(id);

            if (tipoServicio == null)
            {
                return NotFound();
            }

            return Ok(tipoServicio);
        }

        // PUT: api/TipoServicios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoServicio([FromRoute] int id, [FromBody] TipoServicio tipoServicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tipoServicio.TipoServicioId)
            {
                return BadRequest();
            }
            if (_context.Comodidades.Any(c => c.Nombre == tipoServicio.Nombre && tipoServicio.TipoServicioId != id))
            {
                return CreatedAtAction("GetTipoServicio", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(tipoServicio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoServicioExists(id))
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

        // POST: api/TipoServicios
        [HttpPost]
        public async Task<IActionResult> PostTipoServicio([FromBody] TipoServicio tipoServicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Comodidades.Any(c => c.Nombre == tipoServicio.Nombre ))
            {
                return CreatedAtAction("GetTipoServicio", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.TipoServicio.Add(tipoServicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoServicio", new { id = tipoServicio.TipoServicioId }, tipoServicio);
        }

        // DELETE: api/TipoServicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoServicio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoServicio = await _context.TipoServicio.FindAsync(id);
            if (tipoServicio == null)
            {
                return NotFound();
            }

            _context.TipoServicio.Remove(tipoServicio);
            await _context.SaveChangesAsync();

            return Ok(tipoServicio);
        }

        private bool TipoServicioExists(int id)
        {
            return _context.TipoServicio.Any(e => e.TipoServicioId == id);
        }
    }
}