using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrecioServiciosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PrecioServiciosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PrecioServicios
        [HttpGet]
        public IEnumerable<PrecioServicio> GetPrecioServicio()
        {
            return _context.PrecioServicio;
        }

        // GET: api/PrecioServicios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrecioServicio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioServicio = await _context.PrecioServicio.FindAsync(id);

            if (precioServicio == null)
            {
                return NotFound();
            }

            return Ok(precioServicio);
        }

        // PUT: api/PrecioServicios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecioServicio([FromRoute] int id, [FromBody] PrecioServicio precioServicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != precioServicio.PrecioServicioId)
            {
                return BadRequest();
            }
            precioServicio.Temporada = _context.Temporadas.Single(x => x.TemporadaId == precioServicio.Temporada.TemporadaId);
            _context.Entry(precioServicio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioServicioExists(id))
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

        // POST: api/PrecioServicios
        [HttpPost]
        public async Task<IActionResult> PostPrecioServicio([FromBody] PrecioServicio precioServicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            precioServicio.Temporada = _context.Temporadas.Single(x => x.TemporadaId == precioServicio.Temporada.TemporadaId);

            _context.PrecioServicio.Add(precioServicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrecioServicio", new { id = precioServicio.PrecioServicioId }, precioServicio);
        }

        // DELETE: api/PrecioServicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecioServicio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioServicio = await _context.PrecioServicio.FindAsync(id);
            if (precioServicio == null)
            {
                return NotFound();
            }

            _context.PrecioServicio.Remove(precioServicio);
            await _context.SaveChangesAsync();

            return Ok(precioServicio);
        }

        private bool PrecioServicioExists(int id)
        {
            return _context.PrecioServicio.Any(e => e.PrecioServicioId == id);
        }
    }
}