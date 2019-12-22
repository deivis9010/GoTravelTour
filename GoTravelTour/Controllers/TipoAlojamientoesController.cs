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
    public class TipoAlojamientoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TipoAlojamientoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/TipoAlojamientoes
        [HttpGet]
        public IEnumerable<TipoAlojamiento> GetTipoAlojamientos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<TipoAlojamiento> lista;
            if (col == "-1")
            {
                return _context.TipoAlojamientos.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.TipoAlojamientos.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.TipoAlojamientos.ToPagedList(pageIndex, pageSize).ToList();
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

        // GET: api/TipoAlojamientoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetTipoAlojamientoesCount()
        {
            return _context.TipoAlojamientos.Count();
        }

        // GET: api/TipoAlojamientoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoAlojamiento = await _context.TipoAlojamientos.FindAsync(id);

            if (tipoAlojamiento == null)
            {
                return NotFound();
            }

            return Ok(tipoAlojamiento);
        }

        // PUT: api/TipoAlojamientoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTipoAlojamiento([FromRoute] int id, [FromBody] TipoAlojamiento tipoAlojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tipoAlojamiento.TipoAlojamientoId)
            {
                return BadRequest();
            }
     
            if (_context.TipoAlojamientos.Any(c => c.Nombre == tipoAlojamiento.Nombre && id != tipoAlojamiento.TipoAlojamientoId))
            {
                return CreatedAtAction("GetTipoAlojamiento", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(tipoAlojamiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoAlojamientoExists(id))
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

        // POST: api/TipoAlojamientoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostTipoAlojamiento([FromBody] TipoAlojamiento tipoAlojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (_context.TipoAlojamientos.Any(c => c.Nombre == tipoAlojamiento.Nombre))
            {
                return CreatedAtAction("GetTipoAlojamiento", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.TipoAlojamientos.Add(tipoAlojamiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoAlojamiento", new { id = tipoAlojamiento.TipoAlojamientoId }, tipoAlojamiento);
        }

        // DELETE: api/TipoAlojamientoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTipoAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoAlojamiento = await _context.TipoAlojamientos.FindAsync(id);
            if (tipoAlojamiento == null)
            {
                return NotFound();
            }

            _context.TipoAlojamientos.Remove(tipoAlojamiento);
            await _context.SaveChangesAsync();

            return Ok(tipoAlojamiento);
        }

        private bool TipoAlojamientoExists(int id)
        {
            return _context.TipoAlojamientos.Any(e => e.TipoAlojamientoId == id);
        }
    }
}