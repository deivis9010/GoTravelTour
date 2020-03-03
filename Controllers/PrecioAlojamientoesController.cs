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
    public class PrecioAlojamientoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PrecioAlojamientoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PrecioAlojamientoes
        [HttpGet]
        public IEnumerable<PrecioAlojamiento> GetPrecioAlojamiento(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<PrecioAlojamiento> lista;
            if (col == "-1")
            {
                return _context.PrecioAlojamiento
                    .Include(x => x.Habitacion)
                    .Include(x => x.Hotel)
                    .Include(x => x.Temporada)
                    .OrderBy(a => a.Habitacion.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PrecioAlojamiento.Include(x => x.Habitacion)
                    .Include(x => x.Hotel)
                    .Include(x => x.Temporada)
                    .OrderBy(a => a.Habitacion.Nombre)
                    .Where(p => (p.Habitacion.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.PrecioAlojamiento
                    .Include(x => x.Habitacion)
                    .Include(x => x.Hotel)
                    .Include(x => x.Temporada)
                    .OrderBy(a => a.Habitacion.Nombre).ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Habitacion.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Habitacion.Nombre);

                        }

                        break;

                    }


            }

            return lista;

        }
        // GET: api/PrecioAlojamientoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetPrecioAlojamientoesCount()
        {
            return _context.PrecioAlojamiento.Count();
        }

        // GET: api/PrecioAlojamientoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrecioAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioAlojamiento = await _context.PrecioAlojamiento.FindAsync(id);

            if (precioAlojamiento == null)
            {
                return NotFound();
            }

            return Ok(precioAlojamiento);
        }

        // PUT: api/PrecioAlojamientoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecioAlojamiento([FromRoute] int id, [FromBody] PrecioAlojamiento precioAlojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != precioAlojamiento.PrecioAlojamientoId)
            {
                return BadRequest();
            }

            _context.Entry(precioAlojamiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioAlojamientoExists(id))
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

        // POST: api/PrecioAlojamientoes
        [HttpPost]
        public async Task<IActionResult> PostPrecioAlojamiento([FromBody] PrecioAlojamiento precioAlojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PrecioAlojamiento.Add(precioAlojamiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrecioAlojamiento", new { id = precioAlojamiento.PrecioAlojamientoId }, precioAlojamiento);
        }

        // DELETE: api/PrecioAlojamientoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecioAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioAlojamiento = await _context.PrecioAlojamiento.FindAsync(id);
            if (precioAlojamiento == null)
            {
                return NotFound();
            }

            _context.PrecioAlojamiento.Remove(precioAlojamiento);
            await _context.SaveChangesAsync();

            return Ok(precioAlojamiento);
        }

        private bool PrecioAlojamientoExists(int id)
        {
            return _context.PrecioAlojamiento.Any(e => e.PrecioAlojamientoId == id);
        }
    }
}