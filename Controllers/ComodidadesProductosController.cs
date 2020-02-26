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
    public class ComodidadesProductosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ComodidadesProductosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/ComodidadesProductos
        [HttpGet]
        public IEnumerable<ComodidadesProductos> GetComodidadesProductos()
        {
            return _context.ComodidadesProductos;
        }

        // GET: api/ComodidadesProductos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComodidadesProductos([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comodidadesProductos = await _context.ComodidadesProductos.FindAsync(id);

            if (comodidadesProductos == null)
            {
                return NotFound();
            }

            return Ok(comodidadesProductos);
        }

        // PUT: api/ComodidadesProductos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComodidadesProductos([FromRoute] int id, [FromBody] ComodidadesProductos comodidadesProductos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comodidadesProductos.ComodidadesProductosId)
            {
                return BadRequest();
            }

            _context.Entry(comodidadesProductos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComodidadesProductosExists(id))
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

        // POST: api/ComodidadesProductos
        [HttpPost]
        public async Task<IActionResult> PostComodidadesProductos([FromBody] ComodidadesProductos comodidadesProductos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ComodidadesProductos.Add(comodidadesProductos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComodidadesProductos", new { id = comodidadesProductos.ComodidadesProductosId }, comodidadesProductos);
        }

        // DELETE: api/ComodidadesProductos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComodidadesProductos([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comodidadesProductos = await _context.ComodidadesProductos.FindAsync(id);
            if (comodidadesProductos == null)
            {
                return NotFound();
            }

            _context.ComodidadesProductos.Remove(comodidadesProductos);
            await _context.SaveChangesAsync();

            return Ok(comodidadesProductos);
        }

        private bool ComodidadesProductosExists(int id)
        {
            return _context.ComodidadesProductos.Any(e => e.ComodidadesProductosId == id);
        }
    }
}