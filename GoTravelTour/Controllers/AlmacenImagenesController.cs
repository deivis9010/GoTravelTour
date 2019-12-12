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
    public class AlmacenImagenesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public AlmacenImagenesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/AlmacenImagenes
        [HttpGet]
        public IEnumerable<AlmacenImagenes> GetAlmacenImagenes()
        {
            return _context.AlmacenImagenes;
        }

        // GET: api/AlmacenImagenes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlmacenImagenes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var almacenImagenes = await _context.AlmacenImagenes.FindAsync(id);

            if (almacenImagenes == null)
            {
                return NotFound();
            }

            return Ok(almacenImagenes);
        }

        // PUT: api/AlmacenImagenes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAlmacenImagenes([FromRoute] int id, [FromBody] AlmacenImagenes almacenImagenes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != almacenImagenes.AlmacenImagenesId)
            {
                return BadRequest();
            }

            _context.Entry(almacenImagenes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlmacenImagenesExists(id))
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

        // POST: api/AlmacenImagenes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostAlmacenImagenes([FromBody] AlmacenImagenes almacenImagenes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AlmacenImagenes.Add(almacenImagenes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlmacenImagenes", new { id = almacenImagenes.AlmacenImagenesId }, almacenImagenes);
        }

        // DELETE: api/AlmacenImagenes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAlmacenImagenes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var almacenImagenes = await _context.AlmacenImagenes.FindAsync(id);
            if (almacenImagenes == null)
            {
                return NotFound();
            }

            _context.AlmacenImagenes.Remove(almacenImagenes);
            await _context.SaveChangesAsync();

            return Ok(almacenImagenes);
        }

        private bool AlmacenImagenesExists(int id)
        {
            return _context.AlmacenImagenes.Any(e => e.AlmacenImagenesId == id);
        }
    }
}