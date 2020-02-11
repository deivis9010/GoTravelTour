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
    public class ModificadorsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ModificadorsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Modificadors
        [HttpGet]
        public IEnumerable<Modificador> GetModificadores()
        {
            return _context.Modificadores;
        }

        // GET: api/Modificadors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModificador([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modificador = await _context.Modificadores.FindAsync(id);

            if (modificador == null)
            {
                return NotFound();
            }

            return Ok(modificador);
        }

        // PUT: api/Modificadors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModificador([FromRoute] int id, [FromBody] Modificador modificador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != modificador.ModificadorId)
            {
                return BadRequest();
            }

            _context.Entry(modificador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModificadorExists(id))
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

        // POST: api/Modificadors
        [HttpPost]
        public async Task<IActionResult> PostModificador([FromBody] Modificador modificador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Modificadores.Add(modificador);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModificador", new { id = modificador.ModificadorId }, modificador);
        }

        // DELETE: api/Modificadors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModificador([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modificador = await _context.Modificadores.FindAsync(id);
            if (modificador == null)
            {
                return NotFound();
            }

            _context.Modificadores.Remove(modificador);
            await _context.SaveChangesAsync();

            return Ok(modificador);
        }

        private bool ModificadorExists(int id)
        {
            return _context.Modificadores.Any(e => e.ModificadorId == id);
        }
    }
}