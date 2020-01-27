using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using Microsoft.AspNetCore.Authorization;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestriccionesPreciosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RestriccionesPreciosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/RestriccionesPrecios
        [HttpGet]
        public IEnumerable<RestriccionesPrecio> GetRestriccionesPrecios()
        {
            return _context.RestriccionesPrecios;
        }

        // GET: api/RestriccionesPrecios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestriccionesPrecio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restriccionesPrecio = await _context.RestriccionesPrecios.FindAsync(id);

            if (restriccionesPrecio == null)
            {
                return NotFound();
            }

            return Ok(restriccionesPrecio);
        }

        // PUT: api/RestriccionesPrecios/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRestriccionesPrecio([FromRoute] int id, [FromBody] RestriccionesPrecio restriccionesPrecio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restriccionesPrecio.RestriccionesPrecioId)
            {
                return BadRequest();
            }

            _context.Entry(restriccionesPrecio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestriccionesPrecioExists(id))
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

        // POST: api/RestriccionesPrecios
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostRestriccionesPrecio([FromBody] RestriccionesPrecio restriccionesPrecio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RestriccionesPrecios.Add(restriccionesPrecio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRestriccionesPrecio", new { id = restriccionesPrecio.RestriccionesPrecioId }, restriccionesPrecio);
        }

        // DELETE: api/RestriccionesPrecios/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRestriccionesPrecio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restriccionesPrecio = await _context.RestriccionesPrecios.FindAsync(id);
            if (restriccionesPrecio == null)
            {
                return NotFound();
            }

            _context.RestriccionesPrecios.Remove(restriccionesPrecio);
            await _context.SaveChangesAsync();

            return Ok(restriccionesPrecio);
        }

        private bool RestriccionesPrecioExists(int id)
        {
            return _context.RestriccionesPrecios.Any(e => e.RestriccionesPrecioId == id);
        }
    }
}