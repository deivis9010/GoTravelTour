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
    public class ContratoProductoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ContratoProductoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/ContratoProductoes
        [HttpGet]
        public IEnumerable<ContratoProducto> GetContratoProducto()
        {
            return _context.ContratoProducto;
        }

        // GET: api/ContratoProductoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContratoProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contratoProducto = await _context.ContratoProducto.FindAsync(id);

            if (contratoProducto == null)
            {
                return NotFound();
            }

            return Ok(contratoProducto);
        }

        // PUT: api/ContratoProductoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContratoProducto([FromRoute] int id, [FromBody] ContratoProducto contratoProducto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contratoProducto.ContratoProductoId)
            {
                return BadRequest();
            }

            _context.Entry(contratoProducto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContratoProductoExists(id))
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

        // POST: api/ContratoProductoes
        [HttpPost]
        public async Task<IActionResult> PostContratoProducto([FromBody] ContratoProducto contratoProducto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContratoProducto.Add(contratoProducto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContratoProducto", new { id = contratoProducto.ContratoProductoId }, contratoProducto);
        }

        // DELETE: api/ContratoProductoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContratoProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contratoProducto = await _context.ContratoProducto.FindAsync(id);
            if (contratoProducto == null)
            {
                return NotFound();
            }

            _context.ContratoProducto.Remove(contratoProducto);
            await _context.SaveChangesAsync();

            return Ok(contratoProducto);
        }

        private bool ContratoProductoExists(int id)
        {
            return _context.ContratoProducto.Any(e => e.ContratoProductoId == id);
        }
    }
}