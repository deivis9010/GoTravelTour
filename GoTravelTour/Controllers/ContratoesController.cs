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
    public class ContratoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ContratoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Contratoes
        [HttpGet]
        public IEnumerable<Contrato> GetContratos()
        {
            return _context.Contratos;
        }

        // GET: api/Contratoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContrato([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contrato = await _context.Contratos.FindAsync(id);

            if (contrato == null)
            {
                return NotFound();
            }

            return Ok(contrato);
        }

        // PUT: api/Contratoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContrato([FromRoute] int id, [FromBody] Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contrato.ContratoId)
            {
                return BadRequest();
            }

            _context.Entry(contrato).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContratoExists(id))
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

        // POST: api/Contratoes
        [HttpPost]
        public async Task<IActionResult> PostContrato([FromBody] Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Contratos.Add(contrato);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContrato", new { id = contrato.ContratoId }, contrato);
        }

        // DELETE: api/Contratoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContrato([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null)
            {
                return NotFound();
            }

            _context.Contratos.Remove(contrato);
            await _context.SaveChangesAsync();

            return Ok(contrato);
        }

        private bool ContratoExists(int id)
        {
            return _context.Contratos.Any(e => e.ContratoId == id);
        }
    }
}