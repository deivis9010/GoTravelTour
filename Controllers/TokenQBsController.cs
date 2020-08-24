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
    public class TokenQBsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TokenQBsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/TokenQBs
        [HttpGet]
        public IEnumerable<TokenQB> GetTokenQB()
        {
            return _context.TokenQB;
        }

        // GET: api/TokenQBs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTokenQB([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenQB = await _context.TokenQB.FindAsync(id);

            if (tokenQB == null)
            {
                return NotFound();
            }

            return Ok(tokenQB);
        }

        // PUT: api/TokenQBs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTokenQB([FromRoute] int id, [FromBody] TokenQB tokenQB)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tokenQB.TokenQBId)
            {
                return BadRequest();
            }

            _context.Entry(tokenQB).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TokenQBExists(id))
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

        // POST: api/TokenQBs
        [HttpPost]
        public async Task<IActionResult> PostTokenQB([FromBody] TokenQB tokenQB)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TokenQB.Add(tokenQB);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTokenQB", new { id = tokenQB.TokenQBId }, tokenQB);
        }

        // DELETE: api/TokenQBs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTokenQB([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenQB = await _context.TokenQB.FindAsync(id);
            if (tokenQB == null)
            {
                return NotFound();
            }

            _context.TokenQB.Remove(tokenQB);
            await _context.SaveChangesAsync();

            return Ok(tokenQB);
        }

        private bool TokenQBExists(int id)
        {
            return _context.TokenQB.Any(e => e.TokenQBId == id);
        }
    }
}