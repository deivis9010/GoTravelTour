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
    public class DatosPasajeroPrimariosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public DatosPasajeroPrimariosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/DatosPasajeroPrimarios
        [HttpGet]
        public IEnumerable<DatosPasajeroPrimario> GetDatosPasajeroPrimario()
        {
            return _context.DatosPasajeroPrimario;
        }

        // GET: api/DatosPasajeroPrimarios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDatosPasajeroPrimario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datosPasajeroPrimario = await _context.DatosPasajeroPrimario.FindAsync(id);

            if (datosPasajeroPrimario == null)
            {
                return NotFound();
            }

            return Ok(datosPasajeroPrimario);
        }

        // PUT: api/DatosPasajeroPrimarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDatosPasajeroPrimario([FromRoute] int id, [FromBody] DatosPasajeroPrimario datosPasajeroPrimario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != datosPasajeroPrimario.DatosPasajeroPrimarioId)
            {
                return BadRequest();
            }

            _context.Entry(datosPasajeroPrimario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DatosPasajeroPrimarioExists(id))
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

        // POST: api/DatosPasajeroPrimarios
        [HttpPost]
        public async Task<IActionResult> PostDatosPasajeroPrimario([FromBody] DatosPasajeroPrimario datosPasajeroPrimario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DatosPasajeroPrimario.Add(datosPasajeroPrimario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDatosPasajeroPrimario", new { id = datosPasajeroPrimario.DatosPasajeroPrimarioId }, datosPasajeroPrimario);
        }

        // DELETE: api/DatosPasajeroPrimarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatosPasajeroPrimario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datosPasajeroPrimario = await _context.DatosPasajeroPrimario.FindAsync(id);
            if (datosPasajeroPrimario == null)
            {
                return NotFound();
            }

            _context.DatosPasajeroPrimario.Remove(datosPasajeroPrimario);
            await _context.SaveChangesAsync();

            return Ok(datosPasajeroPrimario);
        }

        private bool DatosPasajeroPrimarioExists(int id)
        {
            return _context.DatosPasajeroPrimario.Any(e => e.DatosPasajeroPrimarioId == id);
        }
    }
}