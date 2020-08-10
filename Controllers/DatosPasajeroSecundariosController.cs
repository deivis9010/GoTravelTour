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
    public class DatosPasajeroSecundariosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public DatosPasajeroSecundariosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/DatosPasajeroSecundarios
        [HttpGet]
        public IEnumerable<DatosPasajeroSecundario> GetDatosPasajeroSecundario()
        {
            return _context.DatosPasajeroSecundario;
        }

        // GET: api/DatosPasajeroSecundarios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDatosPasajeroSecundario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datosPasajeroSecundario = await _context.DatosPasajeroSecundario.FindAsync(id);

            if (datosPasajeroSecundario == null)
            {
                return NotFound();
            }

            return Ok(datosPasajeroSecundario);
        }

        // PUT: api/DatosPasajeroSecundarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDatosPasajeroSecundario([FromRoute] int id, [FromBody] DatosPasajeroSecundario datosPasajeroSecundario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != datosPasajeroSecundario.DatosPasajeroSecundarioId)
            {
                return BadRequest();
            }

            _context.Entry(datosPasajeroSecundario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DatosPasajeroSecundarioExists(id))
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

        // POST: api/DatosPasajeroSecundarios
        [HttpPost]
        public async Task<IActionResult> PostDatosPasajeroSecundario([FromBody] DatosPasajeroSecundario datosPasajeroSecundario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DatosPasajeroSecundario.Add(datosPasajeroSecundario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDatosPasajeroSecundario", new { id = datosPasajeroSecundario.DatosPasajeroSecundarioId }, datosPasajeroSecundario);
        }

        // DELETE: api/DatosPasajeroSecundarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatosPasajeroSecundario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datosPasajeroSecundario = await _context.DatosPasajeroSecundario.FindAsync(id);
            if (datosPasajeroSecundario == null)
            {
                return NotFound();
            }

            _context.DatosPasajeroSecundario.Remove(datosPasajeroSecundario);
            await _context.SaveChangesAsync();

            return Ok(datosPasajeroSecundario);
        }

        private bool DatosPasajeroSecundarioExists(int id)
        {
            return _context.DatosPasajeroSecundario.Any(e => e.DatosPasajeroSecundarioId == id);
        }
    }
}