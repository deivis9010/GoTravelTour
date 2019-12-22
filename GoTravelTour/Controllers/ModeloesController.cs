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
    public class ModeloesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ModeloesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Modeloes
        [HttpGet]
        public IEnumerable<Modelo> GetModelos()
        {
            return _context.Modelos;
        }

        // GET: api/Modeloes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModelo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modelo = await _context.Modelos.FindAsync(id);

            if (modelo == null)
            {
                return NotFound();
            }

            return Ok(modelo);
        }

        // PUT: api/Modeloes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutModelo([FromRoute] int id, [FromBody] Modelo modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != modelo.ModeloId)
            {
                return BadRequest();
            }
            if (_context.Modelos.Any(c => c.Nombre == modelo.Nombre && modelo.ModeloId != id))
            {
                return CreatedAtAction("GetModeloes", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(modelo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModeloExists(id))
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

        // POST: api/Modeloes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostModelo([FromBody] Modelo modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Modelos.Any(c => c.Nombre == modelo.Nombre))
            {
                return CreatedAtAction("GetModeloes", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Modelos.Add(modelo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModelo", new { id = modelo.ModeloId }, modelo);
        }

        // DELETE: api/Modeloes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteModelo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modelo = await _context.Modelos.FindAsync(id);
            if (modelo == null)
            {
                return NotFound();
            }

            _context.Modelos.Remove(modelo);
            await _context.SaveChangesAsync();

            return Ok(modelo);
        }

        private bool ModeloExists(int id)
        {
            return _context.Modelos.Any(e => e.ModeloId == id);
        }
    }
}