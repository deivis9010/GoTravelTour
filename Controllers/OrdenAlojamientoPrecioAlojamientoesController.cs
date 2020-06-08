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
    public class OrdenAlojamientoPrecioAlojamientoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public OrdenAlojamientoPrecioAlojamientoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/OrdenAlojamientoPrecioAlojamientoes
        [HttpGet]
        public IEnumerable<OrdenAlojamientoPrecioAlojamiento> GetOrdenAlojamientoPrecioAlojamiento()
        {
            return _context.OrdenAlojamientoPrecioAlojamiento;
        }

        // GET: api/OrdenAlojamientoPrecioAlojamientoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdenAlojamientoPrecioAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenAlojamientoPrecioAlojamiento = await _context.OrdenAlojamientoPrecioAlojamiento.FindAsync(id);

            if (ordenAlojamientoPrecioAlojamiento == null)
            {
                return NotFound();
            }

            return Ok(ordenAlojamientoPrecioAlojamiento);
        }

        // PUT: api/OrdenAlojamientoPrecioAlojamientoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenAlojamientoPrecioAlojamiento([FromRoute] int id, [FromBody] OrdenAlojamientoPrecioAlojamiento ordenAlojamientoPrecioAlojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ordenAlojamientoPrecioAlojamiento.OrdenAlojamientoPrecioAlojamientoId)
            {
                return BadRequest();
            }

            _context.Entry(ordenAlojamientoPrecioAlojamiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenAlojamientoPrecioAlojamientoExists(id))
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

        // POST: api/OrdenAlojamientoPrecioAlojamientoes
        [HttpPost]
        public async Task<IActionResult> PostOrdenAlojamientoPrecioAlojamiento([FromBody] OrdenAlojamientoPrecioAlojamiento ordenAlojamientoPrecioAlojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrdenAlojamientoPrecioAlojamiento.Add(ordenAlojamientoPrecioAlojamiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenAlojamientoPrecioAlojamiento", new { id = ordenAlojamientoPrecioAlojamiento.OrdenAlojamientoPrecioAlojamientoId }, ordenAlojamientoPrecioAlojamiento);
        }

        // DELETE: api/OrdenAlojamientoPrecioAlojamientoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenAlojamientoPrecioAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenAlojamientoPrecioAlojamiento = await _context.OrdenAlojamientoPrecioAlojamiento.FindAsync(id);
            if (ordenAlojamientoPrecioAlojamiento == null)
            {
                return NotFound();
            }

            _context.OrdenAlojamientoPrecioAlojamiento.Remove(ordenAlojamientoPrecioAlojamiento);
            await _context.SaveChangesAsync();

            return Ok(ordenAlojamientoPrecioAlojamiento);
        }

        private bool OrdenAlojamientoPrecioAlojamientoExists(int id)
        {
            return _context.OrdenAlojamientoPrecioAlojamiento.Any(e => e.OrdenAlojamientoPrecioAlojamientoId == id);
        }
    }
}